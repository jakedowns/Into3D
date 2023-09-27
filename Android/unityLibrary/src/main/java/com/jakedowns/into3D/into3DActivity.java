package com.jakedowns.into3D;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.ParcelFileDescriptor;
import android.os.PowerManager;
import android.util.Log;
import android.view.WindowManager;
import android.widget.Toast;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.jakedowns.BrightnessHelper;
import com.unity3d.player.UnityPlayerActivity;

import java.io.File;
import java.util.Arrays;
import java.util.Objects;

public class into3DActivity extends UnityPlayerActivity {

    Toast toast;
    String TAG = "into3D";
    Context context;
    private static final int MY_PERMISSIONS_REQUEST = 1;

    PowerManager powerManager;
    PowerManager.WakeLock wakeLock;
    private static String intentFilePath;
    private static String intentDataString;

    Activity currentActivity;


//    public Context context;
    @Override
    protected void onCreate(Bundle savedInstanceState) {

        context = getApplicationContext();
        currentActivity = this;

        String[] to_request = new String[0];

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WAKE_LOCK)
                != PackageManager.PERMISSION_GRANTED) {

            // add Manifest.permission.WAKE_LOCK to our to_request array
            to_request = Arrays.copyOf(to_request, to_request.length + 1);
            to_request[to_request.length - 1] = Manifest.permission.WAKE_LOCK;

        }else{
            // Permission has already been granted, acquire the wake lock
            Log.d(TAG, "WAKE LOCK GRANTED");
            acquireWakeLock();
        }

        Log.d(TAG, "[Permissions Check] SDK_INT " + android.os.Build.VERSION.SDK_INT);

        /* new permissions checks for Android 13 API level 33+ */
        if(android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.TIRAMISU){
            int perm_check_video = ContextCompat.checkSelfPermission(this, Manifest.permission.READ_MEDIA_VIDEO);
            int perm_check_audio = ContextCompat.checkSelfPermission(this, Manifest.permission.READ_MEDIA_AUDIO);
            int perm_check_images = ContextCompat.checkSelfPermission(this, Manifest.permission.READ_MEDIA_IMAGES);



            Log.d(TAG, "[Permissions Check] Android 13+ Vid/Aud/Imgs " + perm_check_video + " / " + perm_check_audio + " / " + perm_check_images);

            if(perm_check_video != PackageManager.PERMISSION_GRANTED) {
                //requestPermissions(new String[] { Manifest.permission.READ_MEDIA_VIDEO }, REQUEST_READ_INTERNAL_STORAGE);

                // add Manifest.permission.READ_MEDIA_VIDEO to our to_request array
                to_request = Arrays.copyOf(to_request, to_request.length + 1);
                to_request[to_request.length - 1] = Manifest.permission.READ_MEDIA_VIDEO;

            }
            
            // READ_MEDIA_AUDIO
            if(perm_check_audio != PackageManager.PERMISSION_GRANTED) {

                // add Manifest.permission.READ_MEDIA_AUDIO to our to_request array
                to_request = Arrays.copyOf(to_request, to_request.length + 1);
                to_request[to_request.length - 1] = Manifest.permission.READ_MEDIA_AUDIO;
            }
            
            // READ_MEDIA_IMAGES
            if(perm_check_images != PackageManager.PERMISSION_GRANTED) {

                // add Manifest.permission.READ_MEDIA_IMAGES to our to_request array
                to_request = Arrays.copyOf(to_request, to_request.length + 1);
                to_request[to_request.length - 1] = Manifest.permission.READ_MEDIA_IMAGES;
            }
        }else{
            int perm_check_ext_storage = ContextCompat.checkSelfPermission(this, Manifest.permission.READ_EXTERNAL_STORAGE);
            Log.d(TAG, "[Permissions Check] Android <=12 perm_check_ext_storage: " + perm_check_ext_storage);
            if(perm_check_ext_storage != PackageManager.PERMISSION_GRANTED) {

                // add Manifest.permission.READ_EXTERNAL_STORAGE to our to_request array
                to_request = Arrays.copyOf(to_request, to_request.length + 1);
                to_request[to_request.length - 1] = Manifest.permission.READ_EXTERNAL_STORAGE;
            }
        }

        if(ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {

            // add Manifest.permission.WRITE_EXTERNAL_STORAGE to our to_request array
            to_request = Arrays.copyOf(to_request, to_request.length + 1);
            to_request[to_request.length - 1] = Manifest.permission.WRITE_EXTERNAL_STORAGE;
        }

        // Check if the SYSTEM_ALERT_WINDOW permission is granted
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.SYSTEM_ALERT_WINDOW)
                != PackageManager.PERMISSION_GRANTED) {

            // add Manifest.permission.SYSTEM_ALERT_WINDOW to our to_request array
            to_request = Arrays.copyOf(to_request, to_request.length + 1);
            to_request[to_request.length - 1] = Manifest.permission.SYSTEM_ALERT_WINDOW;
        }

        if(to_request.length > 0){
            // request the permissions
            ActivityCompat.requestPermissions(this, to_request, MY_PERMISSIONS_REQUEST);
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            registerActivityLifecycleCallbacks(new MyActivityLifecycleCallbacks());
        }


        // Calls UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);

        // Set the window type to TYPE_APPLICATION_OVERLAY
        getWindow().setType(WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY);

        // Prints debug message to Logcat
        Log.d("OverrideActivity", "onCreate called!");

        // Parse the intent
        checkForIntent();

        context = getApplicationContext();
        Log.d(TAG, "getBrightness " + BrightnessHelper.getBrightness(context));
    }

    void acquireWakeLock()
    {
        PowerManager powerManager = (PowerManager) getSystemService(POWER_SERVICE);
        PowerManager.WakeLock wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "MyApp::WakeLockTag");
        wakeLock.acquire(120*60*1000L /*120 minutes*/);
        Log.e(TAG, "WAKE LOCK ACQUIRED");
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String[] permissions, int[] grantResults) {


        // loop through the permissions and check if they were granted
        for(int i = 0; i < permissions.length; i++){
            if(grantResults[i] == PackageManager.PERMISSION_GRANTED){
                Log.d(TAG, "Permission " + permissions[i] + " was granted");

                if(permissions[i].equals(Manifest.permission.WAKE_LOCK)){
                    // Permission was granted, you can acquire the wake lock
                    acquireWakeLock();
                }
            }else{
                Log.d(TAG, "Permission " + permissions[i] + " was denied");
            }
        }
    }

    public int getBrightness(){
        return BrightnessHelper.getBrightness(getApplicationContext());
    }
    public void setBrightness(int level){
        BrightnessHelper.setBrightness(getApplicationContext(),level);
    }

    public void checkForIntent()
    {
        Log.d(TAG, "[Intent] checking for Intent");
        parseIntent(getIntent());
    }

    private void parseIntent(Intent intent)
    {
        if(intent == null){
            Log.d(TAG, "[Intent] is null");
            return;
        }

        String type = intent.getType();
        Uri url = intent.getData();
        String resolvedType = "none";
        if(url != null){
            resolvedType = getContentResolver().getType(url);
        }


        Log.d(TAG, "[Intent] ${type} ${resolvedType} / " + type + " " + resolvedType);

        String action = intent.getAction();
        Log.d(TAG, "[Intent] Action: "+action);
        String filepath = null;

        Bundle bundle = intent.getExtras();
        if (bundle != null) {
            for (String key : bundle.keySet()) {
                Log.d(TAG, "[Intent] " + key + " : " + (bundle.get(key) != null ? bundle.get(key) : "NULL"));
            }
        }

        intentDataString = intent.getDataString();

        if (Objects.equals(action, Intent.ACTION_VIEW)) {
            Uri data = intent.getData();
            String scheme = data.getScheme();
            if(Objects.equals(scheme, "content")){
                try {
                    Uri uri = (Uri) bundle.get(Intent.EXTRA_STREAM);
                    Log.d(TAG, "got stream uri? " + uri.toString());
                    intentFilePath = uri.toString();
                    return;
                } catch (Exception e){
                    Log.d(TAG, "Error loading content stream");
                }
            }
            filepath = resolveUri(data);
        }else if(Objects.equals(action, Intent.ACTION_SEND)) {
            String extra_text = intent.getStringExtra(Intent.EXTRA_TEXT);
            if(extra_text != null) {
                Log.d(TAG, "[Intent] extra_text "+extra_text);
                Uri uri = Uri.parse(extra_text.trim());
                Log.d(TAG, "[Intent] extra_text -> uri "+uri);
                if (uri.isHierarchical() && !uri.isRelative()) {
                    filepath = resolveUri(uri);
                }else{
                    Log.d(TAG, "[Intent] error resolving extra_text as uri : "+extra_text);
                }
            }else{
                Log.d(TAG, "[Intent] warn: no extra_text");
            }
        } else {
            filepath = intent.getStringExtra("filepath");
            Log.d(TAG, "[Intent] attempting to resolve from stringExtra filepath : "+filepath);
        }
        intentFilePath = filepath;
        Log.d(TAG, "[Intent] got filepath " + intentFilePath);
        Log.d(TAG, "[Intent] got string " + intentDataString);
    }

    private String resolveUri(Uri data)
    {
        String[] stringables = {"http", "https", "rtmp", "rtmps", "rtp", "rtsp", "mms", "mmst", "mmsh", "tcp", "udp"};
        String filepath;
        String scheme = data.getScheme();
        if(Objects.equals(scheme, "file")) {
            filepath = data.getPath();
        }else if(Objects.equals(scheme, "content")) {
            filepath = openContentFd(data);
        }else if(Arrays.asList(stringables).contains(scheme)) {
            filepath = data.toString();
        }else {
            filepath = null;
        }

        if (filepath == null)
            Log.e(TAG, "[Intent] unknown scheme: ${scheme} "+scheme);

        return filepath;
    }

    private String openContentFd(Uri uri) {
        ContentResolver resolver = getApplicationContext().getContentResolver();
        String uriString = uri.toString();
        Log.v(TAG, String.format("[Intent] Resolving content URI: %s", uriString));
        int fd;
        try {
            ParcelFileDescriptor desc = resolver.openFileDescriptor(uri, "r");
            fd = desc.detachFd();
        } catch(Exception e) {
            Log.e(TAG, String.format("[Intent] Failed to open content fd: %s", e.toString()));
            return null;
        }
        // Find out real file path and see if we can read it directly
        try {
            String path = new File("/proc/self/fd/${fd}").getCanonicalPath();
            if (!path.startsWith("/proc") && new File(path).canRead()) {
                Log.v(TAG, String.format("[Intent] Found real file path: %s",path));
                ParcelFileDescriptor.adoptFd(fd).close(); // we don't need that anymore
                return path;
            }
        } catch(Exception e) {
            Log.e(TAG, String.format("[Intent] error opening %s", e.toString()));
        }
        Log.e(TAG, String.format("[Intent] cannot read directly: %s", uri.toString()));
        // Else, pass the fd to mpv
        return String.format("fdclose://%d",fd);
    }

    /*
    private void parseIntentExtras(Bundle extras) {
        //onloadCommands.clear()
        if (extras == null)
            return;

        // Refer to http://mpv-android.github.io/mpv-android/intent.html
        /*if (extras.getByte("decode_mode") == 2.toByte())
        onloadCommands.add(arrayOf("set", "file-local-options/hwdec", "no"))
        if (extras.containsKey("subs")) {
            val subList = extras.getParcelableArray("subs")?.mapNotNull { it as? Uri } ?: emptyList()
            val subsToEnable = extras.getParcelableArray("subs.enable")?.mapNotNull { it as? Uri } ?: emptyList()

            for (suburi in subList) {
                val subfile = resolveUri(suburi) ?: continue
                        val flag = if (subsToEnable.filter { it.compareTo(suburi) == 0 }.any()) "select" else "auto"

                Log.v(TAG, "Adding subtitles from intent extras: $subfile")
                onloadCommands.add(arrayOf("sub-add", subfile, flag))
            }
        }
        if (extras.getInt("position", 0) > 0) {
            val pos = extras.getInt("position", 0) / 1000f
            onloadCommands.add(arrayOf("set", "start", pos.toString()))
        }*--/
    }
    */

    private void showToast(String msg) {
        if(toast != null){
            toast.setText(msg);
            toast.show();
        }
    }

    @SuppressLint("MissingSuperCall")
    @Override
    public void onPause() {
        // don't call super

        //return;
        super.onPause();
    }


    @Override
    public void onBackPressed()
    {
        //showToast("back pressed");
        
        // Instead of calling UnityPlayerActivity.onBackPressed(), this example ignores the back button event
        //super.onBackPressed();
        
        

        Log.d("into3DActivity", "set brightness");
        BrightnessHelper.setBrightness(getApplicationContext(), 0);

    }


}