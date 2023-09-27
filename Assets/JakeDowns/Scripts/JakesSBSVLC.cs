using UnityEngine;
using System;
using System.Threading.Tasks;
using LibVLCSharp;
//using NRKernal;
using System.Collections.Generic;
//using UnityEngine.Device;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;
//using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using NRKernal;
using System.Collections;
/*using System.Diagnostics.Contracts;
using VideoLibrary;
using AngleSharp.Dom;*/
using System.IO;
using System.Net.Http;
/*using UnityEditor;
using Unity.Jobs;
using Unity.Collections;*/
//using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
//using VideoLibrary.Debug;
using UnityEngine.Networking;
using UnityEngine.Video;
//using UnityEngine.PostProcessing;
//using static JakesSBSVLC;
//using static UnityEditor.Experimental.GraphView.GraphView;

using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using VideoLibrary;
using UnityEngine.Assertions;
using Unity.VisualScripting;

public class JakesSBSVLC : MonoBehaviour
{
    [SerializeField]
    public enum VideoMode
    {
        Mono,
        SBSHalf,
        SBSFull,
        TB,
        _360_2D,
        _180_2D,
        _360_3D,
        _180_3D
    }


    private float nextActionTime = 0.0f;
    public float period = 1.0f;

    public JakesRemoteController jakesRemoteController;


    LibVLC libVLC;
    public MediaPlayer mediaPlayer;

    AndroidJavaClass _brightnessHelper;

    [SerializeField]
    GameObject NRCameraRig;
    Camera LeftCamera;
    Camera CenterCamera;
    Camera RightCamera;

    float leftCameraXOnStart;
    float rightCameraXOnStart;

    [SerializeField]
    public MyIAPHandler myIAPHandler;

    GameObject _hideWhenLocked;
    GameObject _lockScreenNotice;
    GameObject _menuToggleButton;
    GameObject _logo;
    /*public GameObject _360Sphere;*/
    /*GameObject _2DDisplaySet;*/

    GameObject _plane2SphereSet;
    GameObject _plane2SphereLeftEye;
    GameObject _plane2SphereRightEye;

    Vector3 _startPosition;

    Renderer _morphDisplayLeftRenderer;
    Renderer _morphDisplayRightRenderer;

    long _prevPlaybackTimeOnBoot;
    bool _needsToRestorePlaybackTime = false;
    int _prevVideoModeOnBoot;

    [SerializeField]
    public Slider fovBar;

    [SerializeField]
    public Slider nrealFOVBar;

    [SerializeField]
    public Slider scaleBar;

    [SerializeField]
    public Slider distanceBar;

    [SerializeField]
    public Slider deformBar;

    [SerializeField]
    public Slider brightnessBar;

    [SerializeField]
    public Slider contrastBar;

    [SerializeField]
    public Slider cbRedBar;

    [SerializeField]
    public Slider cbGreenBar;

    [SerializeField]
    public Slider cbBlueBar;    

    [SerializeField]
    public Slider gammaBar;

    [SerializeField]
    public Slider volumeBar;

    [SerializeField]
    public Slider seekBar;

    [SerializeField]
    public Slider arComboBar;

    /*
    [SerializeField]
    public UnityEngine.UI.Slider saturationBar;

    [SerializeField]
    public UnityEngine.UI.Slider hueBar;
    
    [SerializeField]
    public UnityEngine.UI.Slider sharpnessBar;*/

    [SerializeField]
    public UnityEngine.UI.Slider horizontalBar;

    [SerializeField]
    public UnityEngine.UI.Slider verticalBar;

    [SerializeField]
    public UnityEngine.UI.Slider depthBar;

    [SerializeField]
    public UnityEngine.UI.Slider focusBar;

    [SerializeField]
    public InputField pathTextField;

    GameObject _mainLogo;
    GameObject _pointLight;

    bool _screenLocked = false;
    int _brightnessOnLock = 0;
    int _brightnessModeOnLock = 0;

    bool _flipStereo = false;

    [SerializeField]
    Button _flipStereoButton;

    [SerializeField]
    public VideoMode _videoMode = VideoMode.Mono;// 2d by default

    [SerializeField]
    public JakesVLCPlayerExampleGui jakesVLCPlayerExampleGui;


    /*
    // Flat Left
    [SerializeField]
    public GameObject leftEye;

    // Flat Right
    [SerializeField]
    public GameObject rightEye;

    // Sphere Left
    [SerializeField]
    public GameObject leftEyeSphere;
    // Sphere Right
    [SerializeField]
    public GameObject rightEyeSphere;
    */

    Renderer m_lRenderer;
    Renderer m_rRenderer;

    Renderer m_l360Renderer;
    Renderer m_r360Renderer;

    public Material m_lMaterial;
    public Material m_rMaterial;
    public Material m_monoMaterial;
    public Material m_leftEyeTBMaterial;
    public Material m_rightEyeTBMaterial;

    // deprecated
    /*public Material m_leftEye360Material;
    public Material m_rightEye360Material;*/

    // deprecated
    // TODO: combine 180 and 360 into 2 materials instead of 4?
    /*public Material m_leftEye180Material;
    public Material m_rightEye180Material;*/

    // deprecated
    /*public Material m_3602DSphericalMaterial;
    public Material m_1802DSphericalMaterial;*/

    /// <summary> The NRInput. </summary>
    [SerializeField]
    private NRInput m_NRInput;

    Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
    public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.

    bool _is360 = false;
    bool _is180 = false;

    float Yaw;
    float Pitch;
    float Roll;

    bool _3DModeLocked = true;

    int _3DTrialPlaybackStartedAt = 0;
    float _MaxTrialPlaybackSeconds = 15.0f;
    bool _isTrialing3DMode = false;

    float _aspectRatio;
    bool m_updatedARSinceOpen = false;
    float _aspectRatioOverride;
    string _currentARString;

    /// <summary> The previous position. </summary>
    private Vector2 m_PreviousPos;

    float fov = 20.0f; // 20 for 2D 140 for spherical
    float nreal_fov = 20.0f;

    //public string path = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"; //Can be a web path or a local path
    public string path = "https://jakedowns.com/media/sbs2.mp4"; // Render a nice lil SBS and 180 and 360 video that can play when you switch modes

    public bool flipTextureX = false; //No particular reason you'd need this but it is sometimes useful
    public bool flipTextureY = true; //Set to false on Android, to true on Windows

    public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

    public bool playOnAwake = true; //Open path and Play during Awake

    public bool logToConsole = true; //Log function calls and LibVLC logs to Unity console

    YoutubeClient youtube;

    //JobHandle _handle;
    //DownloadFileJob _jobInstance;

    private float _progress = 0f;
    private bool _isDownloading = false;
    private bool _isDone = false;
    public event Action<float> OnProgress;
    public event Action OnDownloadComplete;
    IEnumerator _downloadCoroutine;
    DownloadOperation _bgDownloadOp;

    YouTubeVideo _mCurrentYoutubeVideo;
    VideoInfo _mCurrentYoutubeVideoInfo;
    string _mCurrentYoutubeVideoCachePath;

    VideoLibrary.Debug.Test _downloaderClassInstance;

    AndroidJavaClass unityPlayer;
    AndroidJavaObject activity;
    AndroidJavaObject context;

    private bool stream_youtube_enabled = true; //false;
    private bool useJobInsteadOfCoroutine = false;
    private bool downloadModeBackground = true;

    // TODO: toggle "coroutine" vs "job" for downloading
    public void ToggleStreamVSDownload()
    {
        stream_youtube_enabled = !stream_youtube_enabled;
        Debug.LogWarning("Stream or download " + (stream_youtube_enabled ? "Stream" : "Download"));
        GameObject.Find("StreamButton").transform.Find("Text").GetComponent<Text>().text = stream_youtube_enabled ? "Stream" : "Download";
    }

    public void ToggleDebugLogging()
    {
        logToConsole = !logToConsole;
        GameObject.Find("toggleDebugLoggingButton").transform.Find("Text").GetComponent<Text>().text = logToConsole ? "Enabled" : "Disabled";
    }

    //Unity Awake, OnDestroy, and Update functions
    #region unity
    void Awake()
    {
        // uncomment if corrupted prefs
        // TODO: track error loading prefs and re-init
        // add a safe mode toggle to reset without needing to get into nested ui
        // ClearPlayerPrefs();

        DebugAudioConfig();

        //Setup LibVLC
        if (libVLC == null)
            CreateLibVLC();

        //Setup Media Player
        CreateMediaPlayer();

        //Setup Youtube Client
        youtube = new YoutubeClient();

        GameObject.Find("StreamButton").transform.Find("Text").GetComponent<Text>().text = stream_youtube_enabled ? "Stream" : "Download";
        GameObject.Find("toggleDebugLoggingButton").transform.Find("Text").GetComponent<Text>().text = logToConsole ? "Enabled" : "Disabled";

#if UNITY_ANDROID
        if (!Application.isEditor)
        {
            GetContext();



            _brightnessHelper = new AndroidJavaClass("com.jakedowns.BrightnessHelper");
            if (_brightnessHelper == null)
            {
                Debug.LogWarning("error loading _brightnessHelper");
            }
        }
#endif

        Debug.Log($"[VLC] LibVLC version and architecture {libVLC.Changeset}");
        Debug.Log($"[VLC] LibVLCSharp version {typeof(LibVLC).Assembly.GetName().Version}");

        _plane2SphereSet = GameObject.Find("NewDisplay");
        _plane2SphereLeftEye = GameObject.Find("plane2sphereLeftEye");
        _plane2SphereRightEye = GameObject.Find("plane2sphereRightEye");

        _startPosition = new Vector3(
            _plane2SphereSet.transform.position.x,
            _plane2SphereSet.transform.position.y,
            _plane2SphereSet.transform.position.z
        );

        /* add event listner to gammaBar input slider */
        if (gammaBar is not null)
        {
            gammaBar.onValueChanged.AddListener(delegate { OnGammaBarValueChanged(); });
        }

        if (brightnessBar is not null)
        {
            brightnessBar.onValueChanged.AddListener(delegate { OnBrightnessBarValueChanged(); });
        }

        if (contrastBar is not null)
        {
            contrastBar.onValueChanged.AddListener(delegate { OnContrastBarValueChanged(); });
        }

        if (cbRedBar is not null)
        {
            cbRedBar.onValueChanged.AddListener(delegate { OnCbRedBarValueChanged(); });
        }

        if (cbGreenBar is not null)
        {
            cbGreenBar.onValueChanged.AddListener(delegate { OnCbGreenBarValueChanged(); });
        }

        if (cbBlueBar is not null)
        {
            cbBlueBar.onValueChanged.AddListener(delegate { OnCbBlueBarValueChanged(); });
        }

        jakesRemoteController.SetJakesSBSVLC(this);

        UpdateCameraReferences();

        leftCameraXOnStart = LeftCamera.transform.position.x;
        rightCameraXOnStart = RightCamera.transform.position.x;

        _mainLogo = GameObject.Find("MainLogo");
        _pointLight = GameObject.Find("Point Light");

        // TODO: extract lockscreen logic into a separate script
        _hideWhenLocked = GameObject.Find("HideWhenScreenLocked");
        _lockScreenNotice = GameObject.Find("LockScreenNotice");
        _logo = GameObject.Find("logo");
        _menuToggleButton = GameObject.Find("MenuToggleButton");

        //Setup Screen
        /*if (screen == null)
            screen = GetComponent<Renderer>();
        if (canvasScreen == null)
            canvasScreen = GetComponent<RawImage>();*/

        _morphDisplayLeftRenderer = _plane2SphereLeftEye.GetComponent<Renderer>();
        _morphDisplayRightRenderer = _plane2SphereRightEye.GetComponent<Renderer>();

        //Automatically flip on android
        if (automaticallyFlipOnAndroid && UnityEngine.Application.platform == RuntimePlatform.Android)
            flipTextureY = !flipTextureY;

        if (UnityEngine.Application.platform != RuntimePlatform.Android)
            flipTextureX = !flipTextureX;

        LoadPreferences();

        //Play On Start
        /*if (playOnAwake)
            Open();*/
    }

    void OnDestroy()
    {
        //Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
        DestroyMediaPlayer();
    }

    void UpdateColorGrade()
    {
        // Get the Color Grading effect from the camera's post-processing profile
        /*ColorGrading colorGrading;
        if (camera.TryGetComponent(out PostProcessVolume volume))
        {
            volume.profile.TryGetSettings(out colorGrading);
        }
        else
        {
            return;
        }*/

        // Set the brightness, contrast, and gamma levels
        /*colorGrading.brightness.value = brightnessBar.value;
        colorGrading.contrast.value = contrastBar.value;
        colorGrading.gamma.value = gammaBar.value;*/
    }

    void Update()
    {

        if ((bool)mediaPlayer?.IsPlaying)
        {
            if (UnityEngine.Time.time > nextActionTime)
            {
                nextActionTime = UnityEngine.Time.time + period;

                SavePrefCurrentPlaybackTime();

                if (_isTrialing3DMode)
                {
                    CheckTrialExceeded();
                }
            }
        }

        if (_isDownloading)
        {
            if (_bgDownloadOp == null)
            {
                Debug.LogWarning("_bgDownloadOp is null");
                _isDownloading = false;
            }
            if (_bgDownloadOp != null)
            {
                Debug.LogWarning($"_bgDownloadOp progress: {_bgDownloadOp.Progress} {_bgDownloadOp.Status} ");
                jakesRemoteController.UpdateDownloadingPopup(_bgDownloadOp.Progress * 100);
                if (_bgDownloadOp.Status == DownloadStatus.Successful)
                {
                    _isDownloading = false;
                    jakesRemoteController.HideDownloadingPopup();
                    mediaPlayer.Media = new Media(_mCurrentYoutubeVideoCachePath);
                    FinalizePlaybackStart();
                }
                else if (_bgDownloadOp.Status == DownloadStatus.Failed)
                {
                    _isDownloading = false;
                    Debug.LogError("failed to download");
                    jakesRemoteController.HideDownloadingPopup();
                    _ShowAndroidToastMessage("error downloading");
                }
                else if (_bgDownloadOp.Status == DownloadStatus.Paused)
                {
                    _isDownloading = false;
                    Debug.LogWarning("bg download op paused?");
                }
                // else, it's pending or running, let it go...
            }
        }

        /*if (_isDownloading)
        {
            //_progress = _handle.GetValue<DownloadFileJob>().progress;
            //OnProgress?.Invoke(_progress);
            Debug.Log(_progress);

            *//*
            if (_handle.IsCompleted)
            {
                _handle.Complete();
                _isDone = true;
                //OnDownloadComplete?.Invoke();
            }
            *//*
        }
        if (_isDone)
        {
            Debug.Log("download complete");
            _isDownloading = false;
            _isDone = false;

            mediaPlayer.Media = new Media(new Uri(_mCurrentYoutubeVideoCachePath)); //
            FinalizePlaybackStart();
        }*/


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
        }
#endif
        //Get size every frame
        uint height = 0;
        uint width = 0;
        mediaPlayer?.Size(0, ref width, ref height);

        //Automatically resize output textures if size changes
        if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
        {
            ResizeOutputTextures(width, height);
        }

        if (_vlcTexture != null)
        {
            //Update the vlc texture (tex)
            var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
            if (updated)
            {
                _vlcTexture.UpdateExternalTexture(texptr);

                //Copy the vlc texture into the output texture, flipped over
                var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
                Graphics.Blit(_vlcTexture, texture, flip, Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
            }
        }
    }
    #endregion

    void OnDisable()
    {
        DestroyMediaPlayer();
    }

    void OnApplicationQuit()
    {
        DestroyMediaPlayer();
    }

    public void Demo3602D()
    {
        //Open("https://streams.videolan.org/streams/360/eagle_360.mp4");
        Open("https://streams.videolan.org/streams/360/kolor-balloon-icare-full-hd.mp4");
        SetVideoMode3602D();
    }

    public void OnGammaBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnBrightnessBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnContrastBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnCbRedBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnCbBlueBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnCbGreenBarValueChanged()
    {
        UpdateMaterialProps();
    }

    public void OnScaleSliderUpdated()
    {
        float newScale = (float)scaleBar.value;
        //_2DDisplaySet.transform.localScale = new Vector3(newScale, newScale, 1.0f);

        _plane2SphereSet.transform.localScale = new Vector3(newScale, newScale, newScale);

        /*_sphereScale = (float)scaleBar.value;
        _360Sphere = GameObject.Find("SphereDisplay");
        Debug.Log("sphere scale " + _sphereScale);
        _360Sphere.transform.localScale = new Vector3(_sphereScale, _sphereScale, _sphereScale);*/

        SavePrefDisplaySettings();
    }

    public void OnDeformSliderUpdated()
    {
        if (deformBar is null)
        {
            return;
        }

        float value = (float)deformBar.value;

        if (_plane2SphereLeftEye is not null)
        {
            _plane2SphereLeftEye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, value);
            _plane2SphereLeftEye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, value);
        }

        if (_plane2SphereRightEye is not null)
        {
            _plane2SphereRightEye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, value);
            _plane2SphereRightEye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, value);
        }

        SavePrefDisplaySettings();
    }

    float lerpDuration = 1; // TODO: dynamic duration based on startValue
    float startValue = 0;
    float endValue = 10;
    IEnumerator lerpLZero;
    IEnumerator lerpLOne;
    IEnumerator lerpRZero;
    IEnumerator lerpROne;
    //float valueToLerp;

    public void TogglePlaneToSphere()
    {
        float current = _plane2SphereLeftEye.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0);
        if (current < 50)
        {
            AnimatePlaneToSphere();
        }
        else
        {
            AnimateSphereToPlane();
        }
    }
    public void AnimatePlaneToSphere()
    {
        DoPlaneSphereLerp(100.0f);
    }

    public void AnimateSphereToPlane()
    {
        DoPlaneSphereLerp(0.0f);
    }

    public void DoPlaneSphereLerp(float _endValue)
    {
        if (lerpLZero is not null)
            StopCoroutine(lerpLZero);

        if (lerpLOne is not null)
            StopCoroutine(lerpLOne);

        if (lerpRZero is not null)
            StopCoroutine(lerpRZero);

        if (lerpROne is not null)
            StopCoroutine(lerpROne);

        endValue = _endValue;

        lerpLZero = LerpPlaneToSphere(_plane2SphereLeftEye.GetComponent<SkinnedMeshRenderer>(), 0);
        lerpLOne = LerpPlaneToSphere(_plane2SphereLeftEye.GetComponent<SkinnedMeshRenderer>(), 1);

        StartCoroutine(lerpLZero);
        StartCoroutine(lerpLOne);

        lerpRZero = LerpPlaneToSphere(_plane2SphereRightEye.GetComponent<SkinnedMeshRenderer>(), 0);
        lerpROne = LerpPlaneToSphere(_plane2SphereRightEye.GetComponent<SkinnedMeshRenderer>(), 1);

        StartCoroutine(lerpRZero);
        StartCoroutine(lerpROne);
    }

    IEnumerator LerpPlaneToSphere(SkinnedMeshRenderer renderer, int ShapeIndex)
    {
        float timeElapsed = 0;
        startValue = renderer.GetBlendShapeWeight(ShapeIndex);
        while (timeElapsed < lerpDuration)
        {
            renderer.SetBlendShapeWeight(ShapeIndex, Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration));
            timeElapsed += UnityEngine.Time.deltaTime;
            yield return null;
        }
        renderer.SetBlendShapeWeight(ShapeIndex, endValue);
    }

    public void OnDistanceSliderUpdated()
    {
        float newDistance = (float)distanceBar.value;
        Debug.LogWarning("OnDistanceSliderUpdated " + newDistance);
        _plane2SphereSet.transform.localPosition = new Vector3(
            _plane2SphereSet.transform.localPosition.x,
            _plane2SphereSet.transform.localPosition.y,
            newDistance
        );
        SavePrefDisplaySettings();
    }

    /* Horizontal (X) axis offset for screen */
    public void OnHorizontalSliderUpdated()
    {
        float newOffset = (float)horizontalBar.value;
        _plane2SphereSet.transform.localPosition = new Vector3(
            newOffset,
            _plane2SphereSet.transform.localPosition.y,
            _plane2SphereSet.transform.localPosition.z
        );
        SavePrefDisplaySettings();
    }

    /* Vertical (Y) axis offset for screen */
    public void OnVerticalSliderUpdated()
    {
        float newOffset = (float)verticalBar.value;
        _plane2SphereSet.transform.localPosition = new Vector3(
            _plane2SphereSet.transform.localPosition.x,
            newOffset,
            _plane2SphereSet.transform.localPosition.z
        );
        SavePrefDisplaySettings();
    }

    /*public void ResetDisplayAdjustments()
    {
        _plane2SphereSet.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _plane2SphereSet.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
    }*/

    float leftCameraMinX = -1.5f;
    float rightCameraMaxX = 1.5f;

    public void OnDepthBarUpdated()
    {
        float newDepth = (float)depthBar.value;

        // move left and right camera closer or further to each other depending on the depthbar value
        // if the value is 0, the cameras are the min distance apart from each other on their local x axis (leftCameraXOnStart / rightCameraXOnStart)
        // if the value is 100, the cameras are at the max distance apart from each other on their local x axis (leftCameraMinX / rightCameraMaxX)

        float leftCameraX = Mathf.Lerp(leftCameraXOnStart, leftCameraMinX, newDepth / 100.0f);
        float rightCameraX = Mathf.Lerp(rightCameraXOnStart, rightCameraMaxX, newDepth / 100.0f);

        Debug.Log($"{newDepth} , {leftCameraX} , {rightCameraX}");

        LeftCamera.transform.localPosition = new Vector3(leftCameraX, LeftCamera.transform.localPosition.y, LeftCamera.transform.localPosition.z);
        RightCamera.transform.localPosition = new Vector3(rightCameraX, RightCamera.transform.localPosition.y, RightCamera.transform.localPosition.z);

        SavePrefDisplaySettings();
    }

    static float maxFocal = 15.0f;
    static float minFocal = -15.0f;

    public void OnFocusBarUpdated()
    {
        float focus = (float)focusBar.value; // percentage 0-100

        /* rotate the left and right camera ever so slightly so that the convergence plane / focus plane changes */
        float focal = Mathf.Lerp(minFocal, maxFocal, focus / 100.0f);
        LeftCamera.transform.localRotation = Quaternion.Euler(0.0f, focal, 0.0f);
        RightCamera.transform.localRotation = Quaternion.Euler(0.0f, -focal, 0.0f);

        SavePrefDisplaySettings();

    }

    public void OnFOVSliderUpdated()
    {
        if (fovBar is null)
        {
            Debug.LogWarning("fovBar null");
            return;
        }
        fov = (float)fovBar.value;
        Debug.Log("fov " + fov);

        Do360Navigation();
        //}
    }

    public void UpdateCameraReferences()
    {
        LeftCamera = GameObject.Find("LeftCamera")?.GetComponent<Camera>();
        CenterCamera = GameObject.Find("CenterCamera")?.GetComponent<Camera>();
        RightCamera = GameObject.Find("RightCamera")?.GetComponent<Camera>();
    }

    public void OnSplitFOVSliderUpdated()
    {
        // NOTE: NRSDK doesn't support custom FOV on cameras
        // NOTE: TESTING COMMENTING OUT camera.projectionMatrix = statements in NRHMDPoseTracker
        //return;


        UpdateCameraReferences();
        if (nrealFOVBar is null)
        {
            Debug.LogWarning("nrealFOVBar null");
            return;
        }

        if (LeftCamera is null || CenterCamera is null || RightCamera is null)
        {
            Debug.LogWarning("camera null " + $" {LeftCamera}, {CenterCamera}, {RightCamera}");
            return;
        }
        //Debug.Log("fov before: " + LeftCamera.fieldOfView + ", " + CenterCamera.fieldOfView + ", " + RightCamera.fieldOfView);

        nreal_fov = (float)nrealFOVBar.value;

        SavePrefDisplaySettings();

        LeftCamera.fieldOfView = nreal_fov;
        CenterCamera.fieldOfView = nreal_fov;
        RightCamera.fieldOfView = nreal_fov;

        //Debug.Log("fov after: " + LeftCamera.fieldOfView + ", " + CenterCamera.fieldOfView + ", " + RightCamera.fieldOfView);

        Do360Navigation();

        //Debug.Log("fov after 360 nav" + LeftCamera.fieldOfView + ", " + CenterCamera.fieldOfView + ", " + RightCamera.fieldOfView);
    }
    public void SetVideoMode1802D()
    {
        SetVideoMode(VideoMode._180_2D);
    }

    public void SetVideoMode3602D()
    {
        SetVideoMode(VideoMode._360_2D);
    }

    public void SetVideoMode1803D()
    {
        SetVideoMode(VideoMode._180_3D);
    }

    public void SetVideoMode3603D()
    {
        SetVideoMode(VideoMode._360_3D);
    }

    public void SetAR4_3()
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = "4:3";
    }

    public void SetAR169()
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = "16:9";
    }

    public void SetAR16_10()
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = "16:10";
    }

    public void SetAR_2_35_to_1()
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = "2.35:1";
    }

    public void SetARNull()
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = null;
    }

    VideoMode[] _SphericalModes = new VideoMode[4] { VideoMode._180_2D, VideoMode._360_2D, VideoMode._180_3D, VideoMode._360_3D };
    private float _sphereScale;

    void OnGUI()
    {
        if (!jakesRemoteController.OGMenuVisible())
        {
            return;
        }
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            m_PreviousPos = NRInput.GetTouch();
        }
        else if (NRInput.GetButton(ControllerButton.TRIGGER))
        {
            //UpdateScroll();
            Do360Navigation();
        }
        else if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            //m_PreviousPos = Vector2.zero;
        }


    }

    void Do360Navigation()
    {
        if(mediaPlayer == null)
        {
            return;
        }
        var range = Math.Max(UnityEngine.Screen.width, UnityEngine.Screen.height);

        Yaw = mediaPlayer.Viewpoint.Yaw;
        Pitch = mediaPlayer.Viewpoint.Pitch;
        Roll = mediaPlayer.Viewpoint.Roll;


        Vector2 deltaMove = NRInput.GetTouch() - m_PreviousPos;
        m_PreviousPos = NRInput.GetTouch();

        float absX = Mathf.Abs(deltaMove.x);
        float absY = Mathf.Abs(deltaMove.y);

        float eighty_or_delta_x = absX > 0 ? absX * 10000 : 80;
        float eighty_or_delta_y = absY > 0 ? absY * 10000 : 80;

        //Debug.Log($"*80x {eighty_or_delta_x} 80y {eighty_or_delta_y} fov {fov} fov2 {nreal_fov}");

        bool? result = null;
        try
        {
            if (Input.GetKey(KeyCode.RightArrow) || deltaMove.x > 0)
            {
                result = mediaPlayer.UpdateViewpoint(Yaw + (float)(eighty_or_delta_x * +40 / range), Pitch, Roll, fov, true);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || deltaMove.x < 0)
            {
                result = mediaPlayer.UpdateViewpoint(Yaw - (float)(eighty_or_delta_x * +40 / range), Pitch, Roll, fov, true);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || deltaMove.y < 0)
            {
                result = mediaPlayer.UpdateViewpoint(Yaw, Pitch + (float)(eighty_or_delta_y * +20 / range), Roll, fov, true);
            }
            else if (Input.GetKey(KeyCode.UpArrow) || deltaMove.y > 0)
            {
                result = mediaPlayer.UpdateViewpoint(Yaw, Pitch - (float)(eighty_or_delta_y * +20 / range), Roll, fov, true);
            }
        } catch (Exception e)
        {
            Debug.LogWarning("error updating viewpoint " + e);
        }

        //Debug.Log("Update Viewpoint Result " + result.ToString());
    }

    //Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.
    #region vlc

    public void Open(byte[] data)
    {
           
    }

    public void OpenFromLocalPath(string path)
    {
        this.path = path;
        SavePrefCurrentMediaSelection();
        Open();
    }
    public void Open(string path)
    {
        if(path == "StreamMediaInput")
        {
            return;
        }
        Debug.LogWarning("VLCPlayerExample Open " + path);
        this.path = path;
        SavePrefCurrentMediaSelection();
        Open();
    }

    public void GetPathFromClipboard()
    {
        //string clipboard_string = GUIUtility.systemCopyBuffer;
        string clipboard_string = UniClipboard.GetText();
        Debug.Log("clipboard_string " + clipboard_string);
        if (pathTextField != null) {

            pathTextField.text = clipboard_string;
        }
        Open(clipboard_string);
    }

    public void GetYoutubeVideoInfo()
    {
        var youTube = YouTube.Default;
        _mCurrentYoutubeVideo = youTube.GetVideo(path);
        string title = _mCurrentYoutubeVideo.Title;
        _mCurrentYoutubeVideoInfo = _mCurrentYoutubeVideo.Info; //(Title, Author, LengthSeconds)
        string fileExtension = _mCurrentYoutubeVideo.FileExtension;
        string fullName = _mCurrentYoutubeVideo.FullName; // same thing as title + fileExtension
        int resolution = _mCurrentYoutubeVideo.Resolution;
        Debug.Log($"[VideoLibrary] title {title} fileExtension {fileExtension} fullName {fullName} resolution {resolution}");
    }

    public async void Open()
    {
        try
        {
            if (mediaPlayer?.Media != null)
                mediaPlayer.Media.Dispose();

            var trimmedPath = this.path.Trim(new char[] { '"' }); //Windows likes to copy paths with quotes but Uri does not like to open them

            Debug.LogWarning("VLCPlayerExample Open: " + trimmedPath);
            // TODO: support playlist urls

            /*if(this.path.Contains("content://")){

                mediaPlayer.Media = new StreamMedia(inputStream);
            }
            else if (trimmedPath.Contains("fdclose"))
            {
                Debug.LogWarning("attempting to open fdclose handle");
                mediaPlayer.Media = new Media(trimmedPath, FromType.FromLocation);

                FinalizePlaybackStart();
            }
            else*/

            const bool useYoutubeExplode = false;

            if (
                useYoutubeExplode && (
                trimmedPath.Contains("://youtube") 
                || trimmedPath.Contains("://www.youtube") 
                || trimmedPath.Contains("://youtu.be")
            ))
            {
                PlayYoutubeLinkUsingYoutubeExplode(trimmedPath);
            }
            else
            {
                try
                {
                    Debug.Log("opening media " + trimmedPath);
                    mediaPlayer.Media = new Media(new Uri(trimmedPath));
                    FinalizePlaybackStart();
                }catch(Exception e)
                {
                    _ShowAndroidToastMessage("Error parsing URL. please try again");
                    Debug.LogWarning("Error parsing URL. please try again " + trimmedPath);
                }
                return;
            }
        }catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void PlayYoutubeLinkUsingYoutubeExplode(string trimmedPath)
    {
        // If the path contains youtube or youtu.be, we assume it's a youtube video
        // TODO: add stream quality selector with HQ option set to "Download Required"
        if (stream_youtube_enabled)
        {
            // libvideo method
            //Stream stream = _mCurrentYoutubeVideo.Stream();
            //mediaPlayer.Media = new Media(new StreamMediaInput(stream));
            //Debug.LogWarning($"stream {stream.GetType()} {stream}");
            //Debug.LogWarning("streaming from " + _mCurrentYoutubeVideo.Uri);

            // youtubeexplode method
            // Get highest quality muxed stream
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(
                trimmedPath
            );
            if (streamManifest == null)
            {
                Debug.LogWarning($"1. No manifest found for url '{trimmedPath}'");
                _ShowAndroidToastMessage($"1. No manifest found for url '{trimmedPath}'");
                return;
            }

            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            if (streamInfo == null)
            {
                Debug.LogWarning($"2. No supported streams in youtube video '{trimmedPath}'");
                _ShowAndroidToastMessage($"2. No supported streams in youtube video '{trimmedPath}'");
                return;
            }

            string uri = UnityWebRequest.UnEscapeURL(streamInfo.Url);

            Debug.LogWarning($"streamInfo {streamInfo.GetType()} {streamInfo.VideoCodec} {streamInfo.Bitrate} {streamInfo.VideoResolution}");

            Debug.LogWarning($"Streaming {streamInfo.Url} \n {uri}");

            mediaPlayer.Media = new Media(uri, FromType.FromLocation);

            FinalizePlaybackStart();
            return;
        }
        else
        {

            jakesRemoteController.ShowDownloadingPopup();
            // TODO: show some Loading UI here while this completes...
            GetYoutubeVideoInfo();

            Debug.LogWarning("attempting to load youtube video " + _mCurrentYoutubeVideo.Uri);
            // if file exists, create a new media uri instead of downloading it again
            _mCurrentYoutubeVideoCachePath = Path.Combine(new string[]
            {
                    Application.persistentDataPath,
                    "ytcache",
                    // url safe title as filename
                    //_mCurrentYoutubeVideo.FullName
                    Regex.Replace(_mCurrentYoutubeVideo.Title, @"[^\w\.@-]", "_"),
                    _mCurrentYoutubeVideo.FileExtension
            });

            // create directory only if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(_mCurrentYoutubeVideoCachePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_mCurrentYoutubeVideoCachePath));
            }

            /*if (File.Exists(_mCurrentYoutubeVideoCachePath)) {
                Debug.Log("loading from disk");
                mediaPlayer.Media = new Media(new Uri(_mCurrentYoutubeVideoCachePath));
                FinalizePlaybackStart();
                return;
            }
            */
            if (_downloadCoroutine != null)
            {
                StopCoroutine(_downloadCoroutine);
            }
            _downloadCoroutine = DownloadAndSaveFile(_mCurrentYoutubeVideo.Uri, _mCurrentYoutubeVideoCachePath);
            StartCoroutine(_downloadCoroutine);
            return;

            //if (true)
            //{
            //_downloaderClassInstance = new VideoLibrary.Debug.Test();
            //_downloaderClassInstance.Run();
            //}
            /*else if (downloadModeBackground)
            {
                _isDownloading = true;
                _isDone = false;
                BackgroundDownloadOptions opts = new BackgroundDownloadOptions(_mCurrentYoutubeVideo.Uri);
                opts.SetDestinationPath(_mCurrentYoutubeVideoCachePath);
                _bgDownloadOp = BackgroundDownloads.StartOrContinueDownload(opts);
                jakesRemoteController.ShowDownloadingPopup();
            }
            else if (useJobInsteadOfCoroutine)
            {
                *//*_jobInstance = new DownloadFileJob
                {
                    uri = new NativeArray<byte>(bytesUri, Allocator.TempJob),
                    filePath = new NativeArray<byte>(bytesFilePath, Allocator.TempJob),
                    progress = new NativeArray<float>(new float[1] { 0.0f }, Allocator.TempJob)
                };
                _handle = _jobInstance.Schedule();*//*
            }
            else
            {
                _isDone = false;
                _isDownloading = true;
                Debug.Log("downloading");
                _downloadCoroutine = DownloadVideo(
                    _mCurrentYoutubeVideo.Uri,
                    _mCurrentYoutubeVideoCachePath
                );
                StartCoroutine(
                    _downloadCoroutine
                );
            }*/

        }

        /*}*/
    }

    IEnumerator DownloadVideo(string uri, string filePath)
    {
        var client = new HttpClient();
        long? totalByte = 0;

        using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
        {
            totalByte = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result.Content.Headers.ContentLength;
            Debug.Log($"[Youtube] downloading stream bytes: {totalByte}");
        }

        using (Task<Stream> streamTask = client.GetStreamAsync(uri))
        using (Stream result = streamTask.Result)
        using (Stream output = File.OpenWrite(filePath))
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            int totalRead = 0;
            Debug.Log($"[Youtube] Download Started {uri} -> {filePath}");
            _progress = 0;
            while ((read = result.Read(buffer, 0, buffer.Length)) > 0)
            {
                jakesRemoteController.ShowDownloadingPopup();
                output.Write(buffer, 0, read);
                totalRead += read;
                _progress = (float)totalRead / (float)totalByte;
                jakesRemoteController.UpdateDownloadingPopup(_progress * 100);
                Debug.Log($"\r [Youtube] Downloading {totalRead}/{totalByte} {_progress} ...");
                yield return null;
            }
            Debug.Log("[Youtube] Download Complete");
            jakesRemoteController.HideDownloadingPopup();
            _progress = 1;
        }
        mediaPlayer.Media = new Media(new Uri(_mCurrentYoutubeVideoCachePath)); //
        FinalizePlaybackStart();
        // yields back as completed
    }
    
    public void CancelYoutubeDownload()
    {
        if(_bgDownloadOp != null)
        {
            BackgroundDownloads.CancelDownload(_bgDownloadOp);
        }
        if(_downloadCoroutine != null)
        {
            StopCoroutine(_downloadCoroutine);
        }
        jakesRemoteController.HideDownloadingPopup();
    }

    /* DownloadFileJob */
    /*IJob DownloadFileJob */

    /*IEnumerator MonitorVideoDownload(JobHandle handle)
    {
        while (!_handle.IsCompleted)
        {
            yield return null;
        }

        _handle.Complete();
        //_handle.Dispose();
        _isDownloading = false;
        _isDone = true;

        mediaPlayer.Media = new Media(new Uri(_mCurrentYoutubeVideoCachePath));
        FinalizePlaybackStart();
    }*/

    /** 
     * walks the {Application.persistenDataPath}/ytcache directory and delete all of the videos in it
     */
    public void ClearYoutubeCache()
    {
        // 1. Loop over all the files in the ytcache folder
        string path = Path.Combine(new string[]
        {
            Application.persistentDataPath,
            "ytcache"
        });
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
        }
    }

    IEnumerator DownloadAndSaveFile(string url, string filePath)
    {
        // Create the UnityWebRequest
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        var downloadHandler = new DownloadHandlerBuffer();
        webRequest.downloadHandler = downloadHandler;

        // Send the request and wait for the response
        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            // Update the progress bar value
            //Debug.LogWarning($"download progress {asyncOperation.progress}");
            jakesRemoteController.UpdateDownloadingPopup( asyncOperation.progress * 100);
            yield return null;
        }

        // Check for errors
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            
            byte[] results = downloadHandler.data;
            File.WriteAllBytes(filePath, results);

            mediaPlayer.Media = new Media(new Uri(filePath));
            FinalizePlaybackStart();
            jakesRemoteController.HideDownloadingPopup();
        }
    }

    async void FinalizePlaybackStart()
    {
        if(mediaPlayer == null)
        {
            Debug.LogWarning("Failed to FinalizePlaybackStart, mediaPlayer is null");
            return;
        }
        if(mediaPlayer.Media == null)
        {
            Debug.LogWarning("mediaPlayer.Media not set");
            return;
        }
        var result = await mediaPlayer.Media.ParseAsync(libVLC, MediaParseOptions.ParseNetwork);
        var trackList = mediaPlayer.Media.TrackList(TrackType.Video);
        bool _is360 = false;

        // if trackList is not empty
        if (trackList != null && trackList.Count > 0)
        {
            _is360 = trackList[0].Data.Video.Projection == VideoProjection.Equirectangular;

            Debug.Log($"projection {trackList[0].Data.Video.Projection}");
            trackList.Dispose();
        }

        // TODO: add SBS / OU / TB / 180 / 360 filename recognition (could do a check of % of repeated pixels in left/right|over/under of sample frames in the video file too)

        /*if (_is360)
            {
                Debug.Log("The video is a 360 video");
                SetVideoMode(VideoMode._360_2D);
            }

            else
            {
                Debug.Log("The video was not identified as a 360 video by VLC");
                SetVideoMode(VideoMode.Mono);
            }*/
        /*}*/

        // flag to read and store the texture aspect ratio
        m_updatedARSinceOpen = false;

        Play();

        LoadPrefCurrentPlaybackTime();

        if (_needsToRestorePlaybackTime)
        {
            Debug.Log($"[restore playback time] {_prevPlaybackTimeOnBoot} / {_prevVideoModeOnBoot} / {_videoMode}");
            _needsToRestorePlaybackTime = false;
            if(_prevPlaybackTimeOnBoot >= 0)
            {
                SetTime(_prevPlaybackTimeOnBoot);
            }
        }

        // delay still needed :G hmm...
        StartCoroutine(SetVideoModeDelayed(1));
        //SetVideoMode(_videoMode);

        SavePrefCurrentMediaSelection();
    }

    IEnumerator SetVideoModeDelayed(int secs)
    {
        Debug.Log("[JakeDowns] SetVideoModeDelayed " + secs + " " + (int)_videoMode);
        yield return new WaitForSeconds(secs);
        //SetVideoModeMono();
        SetVideoMode(_videoMode);
    }

    public void OpenExternal()
    {
        // TODO: Prompt user for path
    }

    public void Play()
    {
        Log("VLCPlayerExample Play");

        if (mediaPlayer == null)
        {
            Debug.LogError("error playing, mediaPlayer not available");
            return;
        }

        jakesVLCPlayerExampleGui.playerControlGroup.SetActive(true);

        _mainLogo?.SetActive(false); // hide logo
        _pointLight?.SetActive(false);

        _plane2SphereSet?.SetActive(true);
        
        

        mediaPlayer.Play();

        var seekbar = GameObject.Find("Seek Bar");
        if(seekbar == null)
        {
            Debug.LogError("Error Locating Seek Bar");
        }
        else{
            var handle = seekbar.transform.Find("Handle Slide Area/Handle");
            if(handle == null)
            {
                Debug.LogError("Error Locating Handle");
            }
            else
            {
                if(handle.transform.position.x is float.NaN)
                {
                    Debug.LogError("fixing broken seek bar!");
                    handle.transform.position = new Vector3(
                        0.0f,
                        handle.transform.position.y,
                        handle.transform.position.z
                    );
                }
            }
            
        }

        CheckTrialExceeded();
    }

    public void Pause()
    {
        Log("VLCPlayerExample Pause");
        mediaPlayer.Pause();
    }

    public void Stop()
    {
        Log("VLCPlayerExample Stop");
        mediaPlayer?.Stop();

        path = null;

        jakesVLCPlayerExampleGui.playerControlGroup.SetActive(false);

        _plane2SphereSet.SetActive(false);

        // TODO: encapsulate this
        if (m_lRenderer?.material is not null)
            m_lRenderer.material.mainTexture = null;

        if (m_rRenderer?.material is not null)
            m_rRenderer.material.mainTexture = null;

        if (m_l360Renderer?.material is not null)
            m_l360Renderer.material.mainTexture = null;

        if (m_r360Renderer?.material is not null)
            m_r360Renderer.material.mainTexture = null;


        // show main logo
        _mainLogo?.SetActive(true);
        _pointLight?.SetActive(true);

        

        // clear to black
        _vlcTexture = null;
        texture = null;

    }

    public void SeekForward10()
    {
        SeekSeconds((float)10);
    }
    
    public void SeekBack10()
    {
        SeekSeconds((float)-10);
    }

    public void SeekSeconds(float seconds)
    {
        Seek((long)seconds * 1000);
    }

    public void Seek(long timeDelta)
    {
        Debug.Log("VLCPlayerExample Seek " + timeDelta);
        mediaPlayer?.SetTime(mediaPlayer.Time + timeDelta);
    }

    public void SetTime(long time)
    {
        Log("VLCPlayerExample SetTime " + time);
        mediaPlayer?.SetTime(time);
    }

    public void SetVolume(int volume = 100)
    {
        Log("VLCPlayerExample SetVolume " + volume);
        mediaPlayer?.SetVolume(volume);
        SavePrefVolume();
    }

    public int Volume
    {
        get
        {
            if (mediaPlayer == null)
                return 0;
            return mediaPlayer.Volume;
        }
    }

    public bool IsPlaying
    {
        get
        {
            if (mediaPlayer == null)
                return false;
            return mediaPlayer.IsPlaying;
        }
    }

    public long Duration
    {
        get
        {
            if (mediaPlayer == null || mediaPlayer.Media == null)
                return 0;
            return mediaPlayer.Media.Duration;
        }
    }

    public long Time
    {
        get
        {
            if (mediaPlayer == null)
                return 0;
            return mediaPlayer.Time;
        }
    }

    public List<MediaTrack> Tracks(TrackType type)
    {
        Log("VLCPlayerExample Tracks " + type);
        MediaTrackList tracklist = mediaPlayer?.Tracks(type);
        if (tracklist == null)
        {
            Log($"VLCPlayerExample Tracklist is null for type {type}");
        }
        else
        {
            Log($"VLCPlayerExample Tracklist count {tracklist.Count} for type {type}");
        }
        return ConvertMediaTrackList(tracklist);
    }

    public MediaTrack SelectedTrack(TrackType type)
    {
        Log("VLCPlayerExample SelectedTrack " + type);
        return mediaPlayer?.SelectedTrack(type);
    }

    public void Select(MediaTrack track)
    {
        Log("VLCPlayerExample Select " + track.Name);
        mediaPlayer?.Select(track);
    }

    public void Unselect(TrackType type)
    {
        Log("VLCPlayerExample Unselect " + type);
        mediaPlayer?.Unselect(type);
    }

    //This returns the video orientation for the currently playing video, if there is one
    public VideoOrientation? GetVideoOrientation()
    {
        var tracks = mediaPlayer?.Tracks(TrackType.Video);

        if (tracks == null || tracks.Count == 0)
            return null;

        var orientation = tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

        return orientation;
    }

    #endregion

    //Private functions create and destroy VLC objects and textures
    #region internal
    //Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
    void CreateLibVLC()
    {
        Log("VLCPlayerExample CreateLibVLC");
        //Dispose of the old libVLC if necessary
        if (libVLC != null)
        {
            libVLC.Dispose();
            libVLC = null;
        }

        Core.Initialize(Application.dataPath); //Load VLC dlls
        libVLC = new LibVLC(enableDebugLogs: true); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

        //Setup Error Logging
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        libVLC.Log += (s, e) =>
        {
            //Always use try/catch in LibVLC events.
            //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
            try
            {
                if (logToConsole)
                {
                    Log(e.FormattedLog);
                }
            }
            catch (Exception ex)
            {
                Log("Exception caught in libVLC.Log: \n" + ex.ToString());
            }

        };
    }

    //Create a new MediaPlayer object and dispose of the old one. 
    void CreateMediaPlayer()
    {
        Log("VLCPlayerExample CreateMediaPlayer");
        if (mediaPlayer != null)
        {
            DestroyMediaPlayer();
        }
        mediaPlayer = new MediaPlayer(libVLC);
        Log("Media Player SET!");
    }

    //Dispose of the MediaPlayer object. 
    void DestroyMediaPlayer()
    {
        if(m_lRenderer?.material is not null)
            m_lRenderer.material.mainTexture = null;

        if(m_rRenderer?.material is not null)
            m_rRenderer.material.mainTexture = null;
        
        if(m_l360Renderer is not null && m_l360Renderer?.material is not null)
            m_l360Renderer.material.mainTexture = null;

        if(m_r360Renderer is not null && m_r360Renderer?.material is not null)
            m_r360Renderer.material.mainTexture = null;

        _vlcTexture = null;

        mediaPlayer?.Stop();
        mediaPlayer?.Dispose();
        mediaPlayer = null;

        libVLC?.Dispose();
        libVLC = null;
        
        Log("JakesSBSVLC DestroyMediaPlayer");
        mediaPlayer?.Stop();
        mediaPlayer?.Dispose();
        mediaPlayer = null;
    }

    //Resize the output textures to the size of the video
    void ResizeOutputTextures(uint px, uint py)
    {
        if(mediaPlayer is null)
        {
            return;
        }
        var texptr = mediaPlayer.GetTexture(px, py, out bool updated);
        if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
        {
            //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
            if (GetVideoOrientation() == VideoOrientation.BottomRight)
            {
                uint swap = px;
                px = py;
                py = swap;
            }

            _vlcTexture = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr); //Make a texture of the proper size for the video to output to
            texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0, RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex

            Debug.Log($"texture size {px} {py} | {_vlcTexture.width} {_vlcTexture.height}");

            if (!m_updatedARSinceOpen)
            {
                m_updatedARSinceOpen = true;
                _aspectRatio = (float)texture.width / (float)texture.height;
                Debug.Log($"[SBSVLC] aspect ratio {_aspectRatio}");
                _currentARString = $"{texture.width}:{texture.height}";
                mediaPlayer.AspectRatio = _currentARString;
                


            }

            if (m_lRenderer != null)
                m_lRenderer.material.mainTexture = texture;
            
            if (m_rRenderer != null)
                m_rRenderer.material.mainTexture = texture;

            /*if (m_l360Renderer != null)
                m_l360Renderer.material.mainTexture = texture;

            if (m_r360Renderer != null)
                m_r360Renderer.material.mainTexture = texture;*/
        }
    }

    //Converts MediaTrackList objects to Unity-friendly generic lists. Might not be worth the trouble.
    List<MediaTrack> ConvertMediaTrackList(MediaTrackList tracklist)
    {
        if (tracklist == null)
            return new List<MediaTrack>(); //Return an empty list

        //Debug.Log("VLCPlayerExample ConvertMediaTrackList " + tracklist.Count);
        var tracks = new List<MediaTrack>((int)tracklist.Count);
        for (uint i = 0; i < (int)tracklist.Count; i++)
        {
            Debug.Log("track found: " + tracklist[i].Name);
            tracks.Add(tracklist[i]);
        }
        Debug.Log("Media Track List Length: " + tracks.Count);
        Assert.IsTrue(tracks.Count == tracklist.Count);
        return tracks;
    }

    public void ToggleFlipStereo()
    {
        _flipStereo = !_flipStereo;
        _flipStereoButton.transform.Find("Text").GetComponent<Text>().text = _flipStereo ? "uncross" : "cross";
        SetVideoMode(_videoMode);
    }

    public string GetCurrentAR()
    {
        return _currentARString;
    }

    public void SetCurrentARString(string ar)
    {
        _currentARString = ar;
    }

    public void UpdateMaterialProps()
    {
        float ar_float = 1.0f;
        
        _currentARString = GetCurrentAR();
        if(_currentARString is not null)
        {
            mediaPlayer.AspectRatio = _currentARString;

            string[] split = _currentARString.Split(':');
            float ar_width = float.Parse(split[0]);
            float ar_height = float.Parse(split[1]);
            ar_float = ar_width / ar_height;
        }
        
        

        float gamma_float = Mathf.Clamp(Mathf.Pow(0.01f, gammaBar.value), -2.0f, 2.0f);
        float brightness_float = brightnessBar.value / 100.0f;
        float contrast_float = contrastBar.value / 100.0f;
        Debug.Log($"[Gamma] {gamma_float} [Brightness] {brightness_float} [Contrast] {contrast_float} [ARCombo] {ar_float}");

        // Store Picture Settings
        PlayerPrefs.SetFloat("Gamma", Mathf.Clamp(gammaBar.value, -2.0f, 2.0f));
        PlayerPrefs.SetFloat("Brightness", Mathf.Clamp(brightnessBar.value, 0.0f, 100.0f));
        PlayerPrefs.SetFloat("Contrast", Mathf.Clamp(contrastBar.value, 0.0f, 100.0f));
        PlayerPrefs.SetFloat("ARCombo", Mathf.Clamp(ar_float, 0.0f, 100.0f));
        PlayerPrefs.SetFloat("ColorBalRed", Mathf.Clamp(cbRedBar.value, -1.0f, 1.0f));
        PlayerPrefs.SetFloat("ColorBalGreen", Mathf.Clamp(cbRedBar.value, -1.0f, 1.0f));
        PlayerPrefs.SetFloat("ColorBalBlue", Mathf.Clamp(cbRedBar.value, -1.0f, 1.0f));

        if (_morphDisplayLeftRenderer?.material is not null)
        {
            _morphDisplayLeftRenderer.material.SetFloat("AspectRatio", Mathf.Clamp(ar_float, 0.0f, 200.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_Gamma", Mathf.Clamp(gamma_float, 0.0f, 200.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_Brightness", Mathf.Clamp(brightness_float, 0.0f, 200.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_Contrast", Mathf.Clamp(contrast_float, 0.0f, 200.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_CyanRed", Mathf.Clamp(cbRedBar.value, -1.0f, 1.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_MagentaGreen", Mathf.Clamp(cbBlueBar.value, -1.0f, 1.0f));
            _morphDisplayLeftRenderer.material.SetFloat("_YellowBlue", Mathf.Clamp(cbGreenBar.value, -1.0f, 1.0f));
        }

        // todo: make a combined shader
        if (_morphDisplayRightRenderer?.material is not null)
        {
            _morphDisplayRightRenderer.material.SetFloat("AspectRatio", Mathf.Clamp(ar_float, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_Gamma", Mathf.Clamp(gamma_float, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_Brightness", Mathf.Clamp(brightness_float, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_Contrast", Mathf.Clamp(contrast_float, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_CyanRed", Mathf.Clamp(cbRedBar.value, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_MagentaGreen", Mathf.Clamp(cbBlueBar.value, 0.0f, 200.0f));
            _morphDisplayRightRenderer.material.SetFloat("_YellowBlue", Mathf.Clamp(cbGreenBar.value, 0.0f, 200.0f));
        }
    }

    public bool GetExceededTrial()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        //Debug.Log("trial exceeded? " + $"cur_time {cur_time} start {_3DTrialPlaybackStartedAt} diff {cur_time - _3DTrialPlaybackStartedAt} v {_MaxTrialPlaybackSeconds}");
        bool trialExceeded = _3DTrialPlaybackStartedAt == 0 ? false : (cur_time - _3DTrialPlaybackStartedAt) > _MaxTrialPlaybackSeconds;
        return trialExceeded;
    }

    public bool CheckTrialExceeded()
    {
        //Debug.Log("CheckTrialExceeded _3DModeLocked?" + _3DModeLocked);
        if (!_3DModeLocked)
        {
            return false;
        }

        bool trialExceeded = GetExceededTrial();
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        //Debug.Log("CheckTrialExceeded trialExceeded?" + trialExceeded);
        //Debug.Log("CheckTrialExceeded video mode?" + _videoMode);
        bool deformedPastFlat = deformBar is null ? false : deformBar.value > 0.1;
        //Debug.Log("CheckTrialExceeded deformedPastFlat? " + deformBar?.value);

        if (
            _videoMode == VideoMode._180_3D 
            || _videoMode == VideoMode._360_3D 
            || (
                deformedPastFlat 
                && (
                    _videoMode == VideoMode.SBSHalf
                    || _videoMode == VideoMode.TB
                )
            )
        )   
        {
            if (trialExceeded)
            {
                jakesRemoteController.ShowUnlock3DSphereModePropmptPopup();
                _videoMode = VideoMode.SBSHalf;
                Debug.Log("CheckTrialExceeded PAUSE!!!");
                Pause();
            } 
            else
            {
                if(_3DTrialPlaybackStartedAt == 0 && mediaPlayer.IsPlaying){
                    _3DTrialPlaybackStartedAt = cur_time;
                    _isTrialing3DMode = true;
                }
            }
        }
        return trialExceeded;
    }

    public void ClearMaterialTextureLinks()
    {
        if (_morphDisplayLeftRenderer.material is not null)
        {
            _morphDisplayLeftRenderer.material.mainTexture = null;
            _morphDisplayLeftRenderer.material = null;
        }

        if (_morphDisplayRightRenderer.material is not null)
        {
            _morphDisplayRightRenderer.material.mainTexture = null;
            _morphDisplayRightRenderer.material = null;
        }
    }

    public void SetVideoMode(VideoMode mode)
    {
        _videoMode = mode;
        CheckTrialExceeded();
        Debug.Log($"[JakeDowns] set video mode {(int)mode} {mode}");

        SavePrefFormat();

        //flipTextureX = false;

        ClearMaterialTextureLinks();

        if(texture == null)
        {
            Debug.LogWarning("[SetVideoMode] texture is null!");
        }

        if(mode == VideoMode.Mono || mode == VideoMode._360_2D || mode == VideoMode._180_2D)
        {
            // 2D
            _plane2SphereLeftEye.layer = LayerMask.NameToLayer("Default");
            _plane2SphereRightEye.SetActive(false);

            _morphDisplayLeftRenderer.material = m_monoMaterial; // m_lMaterial;
            _morphDisplayLeftRenderer.material.mainTexture = texture;
        }
        else
        {
            // 3D

            _plane2SphereLeftEye.layer = LayerMask.NameToLayer("LeftEyeOnly");

            _plane2SphereRightEye.SetActive(true);
            _plane2SphereRightEye.layer = LayerMask.NameToLayer("RightEyeOnly");

            if (mode is VideoMode.TB)
            {
                _morphDisplayLeftRenderer.material = _flipStereo ? m_rightEyeTBMaterial : m_leftEyeTBMaterial;
                _morphDisplayRightRenderer.material = _flipStereo ? m_leftEyeTBMaterial : m_rightEyeTBMaterial;
            }
            else
            {
                _morphDisplayLeftRenderer.material = _flipStereo ? m_rMaterial : m_lMaterial;
                _morphDisplayRightRenderer.material = _flipStereo ? m_lMaterial : m_rMaterial;
            }

            _morphDisplayLeftRenderer.material.mainTexture = texture;
            _morphDisplayRightRenderer.material.mainTexture = texture;
        }

        UpdateMaterialProps();
        
    }

    public void ShowCustomARPopup()
    {
        jakesRemoteController.ShowPopupByID(JakesRemoteController.PopupID.CUSTOM_AR_POPUP);
    }

    public void SetAspectRatio(string value)
    {
        if (mediaPlayer is not null)
            mediaPlayer.AspectRatio = value;
    }

    // https://answers.unity.com/questions/1549639/enum-as-a-function-param-in-a-button-onclick.html?page=2&pageSize=5&sort=votes

    public void SetVideoModeMono() => SetVideoMode(VideoMode.Mono);
    public void SetVideoModeSBSHalf() => SetVideoMode(VideoMode.SBSHalf);
    public void SetVideoModeSBSFull() => SetVideoMode(VideoMode.SBSFull);
    public void SetVideoModeTB() => SetVideoMode(VideoMode.TB);

    public void ResetScreen()
    {
        _plane2SphereLeftEye.transform.localPosition = _startPosition;
        _plane2SphereLeftEye.transform.localRotation = Quaternion.identity;
        _plane2SphereLeftEye.transform.localScale = new Vector3(1, 1, 1);

        _plane2SphereRightEye.transform.localPosition = _startPosition;
        _plane2SphereRightEye.transform.localRotation = Quaternion.identity;
        _plane2SphereRightEye.transform.localScale = new Vector3(1, 1, 1);
    }

    public void LoadViaSMB()
    {
        /*libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
        (dialog, title, text, username, store, token) =>
        {
            dialog.PostLogin(Username, Password, false);
            tcs.SetResult(true);
            return Task.CompletedTask;
        },
        (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
        (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
        (dialog, position, text) => Task.CompletedTask);*/

        //Media media = new Media(libVLC, UrlRequireAuth, Media.FromType.FromLocation);
    }

    public void promptUserFilePicker()
    { 

#if UNITY_EDITOR
        string[] fileTypes = new string[] { "*" };
# elif UNITY_ANDROID
        // Use MIMEs on Android
        string[] fileTypes = new string[] { "video/*" };
#else
		// Use UTIs on iOS
		string[] fileTypes = new string[] { "public.movie" };
#endif
        
        // Pick image(s) and/or video(s)
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                Debug.Log("Picked file: " + path);
                Open(path);
            }
        }, fileTypes);

        if (permission is not NativeFilePicker.Permission.Granted)
        {
            _ShowAndroidToastMessage($"Permission result: {permission}");
            Debug.Log("Permission result: " + permission);
        }
    }

   

    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        /*AndroidJavaClass unityPlayer = new AndroidJavaClass("com.jakedowns.into3D.VLC3DActivity");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");*/

        /*AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");*/

        try
        {

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (unityActivity != null)
                {
                    AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                    unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                        toastObject.Call("show");
                    }));
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }


    }

    public void OnSingleTap(string name)    
    {
        Debug.Log($"[SBSVLC] Single Tap Triggered {name}");
        if (name == "LockScreenButton")
        {
            if (!_screenLocked)
            {
                ToggleScreenLock();
            }
        }
    }

    // we require a double-tap to unlock
    public void OnDoubleTap(string name)
    {
        Debug.Log($"[SBSVLC] Double Tap Triggered {name}");
        if (name == "LockScreenButton")
        {
            if (_screenLocked)
            {
                ToggleScreenLock();
            }
        }
    }

    void GetContext()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        try
        {
            activity = unityPlayer?.GetStatic<AndroidJavaObject>("currentActivity");
            context = activity?.GetStatic<AndroidJavaObject>("context");
        }
        catch (Exception e)
        {
            Debug.Log("error getting context " + e.ToString());
        }
    }

    public void ToggleScreenLock()
    {
        _screenLocked = !_screenLocked;

        if (_screenLocked)
        {
            // Hide All UI except for the lock button
            _hideWhenLocked.SetActive(false);
            _lockScreenNotice.SetActive(true);
            _logo.SetActive(false);
            _menuToggleButton.SetActive(false);
            // Lower Brightness
            float _unityBrightnessOnLock = Screen.brightness;
            Debug.Log($"lockbrightness Unity brightness on lock {_unityBrightnessOnLock}");

#if UNITY_ANDROID
            if (!Application.isEditor)
            {
                try
                {
                    // get int from _brightnessHelper
                    _brightnessOnLock = (int)(_brightnessHelper?.CallStatic<int>("getBrightness"));

                    _brightnessModeOnLock = (int)(_brightnessHelper?.CallStatic<int>("getBrightnessMode"));

                    Debug.Log($"lockbrightness Android brightness on lock {_brightnessOnLock}");
                }catch(Exception e)
                {
                    Debug.LogWarning("Error getting brightness " + e.ToString());
                }

                // Set it to 0? 0.1?
                //Debug.Log($"set brightness with unity");
                //Screen.brightness = 0.1f;

                if (context is null)
                {
                    Debug.LogWarning("context is null");
                    GetContext();
                }
                if (context is null)
                {
                    Debug.LogWarning("context is still null");
                }
                if (context is not null)
                {
                    // TODO: maybe try to fetch it again now?

                    object _args = new object[2] { context, 1 };

                    // call _brightnessHelper
                    _brightnessHelper?.CallStatic("SetBrightness", _args);
                }
                 

            }
#endif
        }
        else
        {
#if UNITY_ANDROID
            if (!Application.isEditor)
            {
                if (context is null)
                {
                    Debug.Log("context is null");
                    GetContext();
                }
                if (context is not null)
                {
                    try
                    {
                        object _args = new object[2] { context, _brightnessOnLock };
                        _brightnessHelper?.CallStatic("setBrightness", _args);

                        // restore brightness mode
                        object _args_mode = new object[2] { context, _brightnessModeOnLock };
                        _brightnessHelper?.CallStatic("setBrightnessMode", _args_mode);
                    }
                    catch(Exception e)
                    {
                        Debug.Log("error setting brightness " + e.ToString());
                    }
                    
                }
                
            }
#else
            // Restore Brightness
            Screen.brightness = _brightnessOnLock;
#endif

            // Show All UI when screen is unlocked
            _hideWhenLocked.SetActive(true);
            _lockScreenNotice.SetActive(false);
            _logo.SetActive(true);
            _menuToggleButton.SetActive(true);
        }
    }

    void Log(object message)
    {
        if (logToConsole)
            Debug.Log($"[VLC] {message}");
    }
#endregion

    public void Unlock3DMode()
    {
        _3DModeLocked = false;
    }

    public void SavePreferences()
    {
        // media
        SavePrefCurrentMediaSelection();

        // current playback time
        SavePrefCurrentPlaybackTime();

        // volume
        SavePrefVolume();

        // subtitle selection
        // audio track selection
    }

    public void SavePrefCurrentMediaSelection()
    {
        // save current media selection
        PlayerPrefs.SetString("currentMediaSelection", path);
    }

    /*public void LoadPrefCurrentMediaSelection()
    {
        int videoModeInt = PlayerPrefs.GetInt("currentMediaFormat", 0);
        // convert int to enum value
        _videoMode = (VideoMode)videoModeInt;

        _flipStereo = PlayerPrefs.GetInt("currentMediaFlipped", 0) == 1;
        // update text label
        _flipStereoButton.transform.Find("Text").GetComponent<Text>().text = _flipStereo ? "uncross" : "cross";


        // load current media selection
        path = PlayerPrefs.GetString("currentMediaSelection");
        // check path is not null and not an empty string (trimmed)
        if (path != null && path.Trim().Length > 0)
        {
            _needsToRestorePlaybackTime = true; // flag to restore time after the media has booted
            Open(path);
        }
    }*/

    public void SavePrefFormat()
    {
        Debug.LogWarning($"[SavePrefFormat] {(int)_videoMode} {_videoMode}");
        PlayerPrefs.SetInt("currentMediaFormat", (int)_videoMode);
        // currentMediaFlipped
        PlayerPrefs.SetInt("currentMediaFlipped", _flipStereo ? 1 : 0);
    }

    public void SavePrefVolume()
    {
        // save volume
        PlayerPrefs.SetInt("volume", mediaPlayer.Volume);
    }

    public void LoadPrefVolume()
    {
        /*if (mediaPlayer is null) {
            Debug.LogWarning("unable to restore volume, mediaPlayer missing")
            return;
        }*/
        // load volume
        //mediaPlayer.SetVolume(PlayerPrefs.GetInt("volume", 100));
        volumeBar.value = PlayerPrefs.GetInt("volume", 100); // mediaPlayer.Volume;
    }

    public void SavePrefCurrentPlaybackTime()
    {
        if (mediaPlayer != null)    
        {
            long _currentPlaybackTime = mediaPlayer.Time;
            PlayerPrefs.SetString("currentPlaybackTime", _currentPlaybackTime.ToString());
            //Debug.Log($"[VLC] Saved current playback time {_currentPlaybackTime}");
        }
    }

    /*
     * saves display setting slider values to PlayerPrefs
     * 
     */
    public void SavePrefDisplaySettings()
    {
        Debug.LogWarning("saving display pref settings");
        PlayerPrefs.SetFloat("display_horizontal", horizontalBar.value);
        PlayerPrefs.SetFloat("display_vertical", verticalBar.value);
        PlayerPrefs.SetFloat("display_distance", distanceBar.value);
        PlayerPrefs.SetFloat("display_scale", scaleBar.value);
        PlayerPrefs.SetFloat("display_deform", deformBar.value);
        PlayerPrefs.SetFloat("display_nrealFOV", nrealFOVBar.value);
        PlayerPrefs.SetFloat("display_depth", depthBar.value);
        PlayerPrefs.SetFloat("display_focus", focusBar.value);
        //DebugPrintPreferences();
    }

    public void DebugAudioConfig()
    {
        var config = AudioSettings.GetConfiguration();
        // list the available fields and properties and getters on the config
        // Print the audio configuration to the console
        string debug_string = "Audio configuration:";
        debug_string += "\n  Sample rate: " + config.sampleRate;
        debug_string += "\n  Speaker mode: " + config.speakerMode;
        debug_string += "\n  DSP buffer size: " + config.dspBufferSize;

        var SpatializerPluginName = AudioSettings.GetSpatializerPluginName();
        debug_string += "\n  Spatializer plugin name: " + SpatializerPluginName;

#if UNITY_EDITOR
        var SPNames = AudioSettings.GetSpatializerPluginNames();
        if (SPNames != null)
        {
            foreach (var SPName in SPNames)
            {
                debug_string += "\n Available Spatializer plugin name: " + SPName;
            }
        }
#endif

        // log driver capabilities
        debug_string += "\n  Driver capabilities: " + AudioSettings.driverCapabilities;

        Debug.Log(debug_string);
    }

    public void LoadPrefCurrentPlaybackTime()
    {
        if (mediaPlayer != null)
        {
            float _currentPlaybackTime = long.Parse(PlayerPrefs.GetString("currentPlaybackTime", "0"));
            //mediaPlayer.Time = _currentPlaybackTime;
            seekBar.value = Mathf.Clamp(_currentPlaybackTime / mediaPlayer.Length, 0, 1);
            Debug.Log($"[VLC] Loaded current playback time {_currentPlaybackTime}");
        }
        else
        {
            Debug.LogError("[VLC] failed to restore playback time, mediaPlayer is null");
        }
    }

    public void DebugPrintPreferences()
    {
        string prefFlipped = PlayerPrefs.GetInt("currentMediaFlipped", 0) > 0 ? "flipped" : "un-flipped";
        string outputString = "";
        outputString += "\n Current User Preferences...";
        outputString += $"\n currentMediaSelection: {PlayerPrefs.GetString("currentMediaSelection")}";
        outputString += $"\n currentMediaFormat: {PlayerPrefs.GetInt("currentMediaFormat", 0)}";
        outputString += $"\n currentMediaFlipped: {prefFlipped}";
        outputString += $"\n currentPlaybackTime: {long.Parse(PlayerPrefs.GetString("currentPlaybackTime", "0"))}";

        outputString += $"\n volume: {PlayerPrefs.GetInt("volume", 100)}";

        outputString += $"\n Gamma: {PlayerPrefs.GetFloat("Gamma", 0.05f)}";
        outputString += $"\n Brightness: {PlayerPrefs.GetFloat("Brightness", 0)}";
        outputString += $"\n Contrast: {PlayerPrefs.GetFloat("Contrast", 0)}";

        outputString += $"\n ARCombo: {PlayerPrefs.GetFloat("ARCombo", 0)}";

        outputString += $"\n display_horizontal: {PlayerPrefs.GetFloat("display_horizontal", 0.0f)}";
        outputString += $"\n display_vertical: {PlayerPrefs.GetFloat("display_vertical", 0.0f)}";
        outputString += $"\n display_distance: {PlayerPrefs.GetFloat("display_distance", 1.0f)}";
        outputString += $"\n display_scale: {PlayerPrefs.GetFloat("display_scale", 1.0f)}";
        outputString += $"\n display_deform: {PlayerPrefs.GetFloat("display_deform", 0.0f)}";
        outputString += $"\n display_nrealFOV: {PlayerPrefs.GetFloat("display_nrealFOV", 20.0f)}";
        outputString += $"\n display_depth: {PlayerPrefs.GetFloat("display_depth", 50.0f)}";
        outputString += $"\n display_focus: {PlayerPrefs.GetFloat("display_focus", 50.0f)}";

        Debug.LogWarning(outputString);
    }

    public void LoadPreferences()
    {
        _prevPlaybackTimeOnBoot = long.Parse(PlayerPrefs.GetString("currentPlaybackTime", "0"));
        _prevVideoModeOnBoot = PlayerPrefs.GetInt("currentMediaFormat", 0);

        // Debug print pref values
        DebugPrintPreferences();

        LoadPrefVolume();
        //LoadPrefCurrentMediaSelection(); // also loads format settings and playback time
        LoadPrefPictureSettings();
        LoadPrefDisplaySettings();
    }

    public void LoadPrefPictureSettings()
    {
        gammaBar.value = PlayerPrefs.GetFloat("Gamma", 0.05f);
        brightnessBar.value = PlayerPrefs.GetFloat("Brightness", 100.0f);
        contrastBar.value = PlayerPrefs.GetFloat("Contrast", 100.0f);

        cbRedBar.value = PlayerPrefs.GetFloat("ColorBalRed", 0.0f);
        cbGreenBar.value = PlayerPrefs.GetFloat("ColorBalGreen", 0.0f);
        cbBlueBar.value = PlayerPrefs.GetFloat("ColorBalBlue", 0.0f);

        arComboBar.value = PlayerPrefs.GetFloat("ARCombo", 1.0f);

        Debug.Log($"[Into3D] Loaded Picture Preferences {gammaBar.value} {brightnessBar.value} {contrastBar.value} {arComboBar.value}");
        UpdateMaterialProps();
    }

    public void LoadPrefDisplaySettings()
    {
        horizontalBar.value = PlayerPrefs.GetFloat("display_horizontal", 0.0f);
        verticalBar.value = PlayerPrefs.GetFloat("display_vertical", 0.0f);
        distanceBar.value = PlayerPrefs.GetFloat("display_distance", 24.49f);
        scaleBar.value = PlayerPrefs.GetFloat("display_scale", 1.0f);
        deformBar.value = PlayerPrefs.GetFloat("display_deform", 0.0f);
        nrealFOVBar.value = PlayerPrefs.GetFloat("display_nrealFOV", 20.0f);
        depthBar.value = PlayerPrefs.GetFloat("display_depth", 50.0f);
        focusBar.value = PlayerPrefs.GetFloat("display_focus", 50.0f);
    }

    public void ResetDisplayPrefs()
    {
        horizontalBar.value = 0;
        verticalBar.value = 0;
        distanceBar.value = 24.49f;
        scaleBar.value = 1;
        deformBar.value = 0;
        nrealFOVBar.value = 20;
        depthBar.value = 50;
        focusBar.value = 50;
    }

    public void ResetPicturePrefs()
    {
        gammaBar.value = 0.05f;
        brightnessBar.value = 100.0f;
        contrastBar.value = 100.0f;

        cbRedBar.value = 0.0f;
        cbGreenBar.value = 0.0f;
        cbBlueBar.value = 0.0f;
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    /*public void ResetFormatPrefs()
    {
        
    }*/
}
