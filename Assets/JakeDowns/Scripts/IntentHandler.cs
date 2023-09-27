using AngleSharp.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Networking;

public class IntentHandler : MonoBehaviour
{
    public JakesSBSVLC jakesSBSVLC;

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogWarning("Hello from my intent handler!");
        OnIntent();
    }

    // OnApplicationFocus
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            OnIntent();
        }
        else
        {
            //Debug.Log("Application lost focus");
        }
    }

    // OnIntent
    void OnIntent()
    {
        if (Application.isEditor)
        {
            return;
        }

        CheckForIntentFile();
    }

    private void CheckForIntentFile()
    {
        string intentFile = null;
        string intentDataString = null;
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject into3DActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (into3DActivity != null)
                {
                    into3DActivity.Call("checkForIntent");

                    intentFile = into3DActivity.GetStatic<String>("intentFilePath");
                    intentDataString = into3DActivity.GetStatic<String>("intentDataString");

                    Debug.LogWarning("[Intent] intentDataString: " + (intentDataString != null ? intentDataString : "none"));

                    if (intentFile != null)
                    {
                        Debug.LogWarning($"opening intent file {intentFile}");
                        intentFile = UnityWebRequest.UnEscapeURL(intentFile);
                        jakesSBSVLC.OpenFromLocalPath(intentFile);
                        /*
                        

                        using (AndroidJavaObject cr = unityActivity.Call<AndroidJavaObject>("getContentResolver"))
                        {
                            using (AndroidJavaObject pfd = cr.Call<AndroidJavaObject>("openFileDescriptor", intentFile, "r"))
                            {
                                if (pfd != null)
                                {
                                    // You can use the pfd to create a FileInputStream
                                    using (AndroidJavaObject fis = new AndroidJavaObject("java.io.FileInputStream", pfd.Call<AndroidJavaObject>("getFileDescriptor")))
                                    {
                                        using (Stream stream = new FileStream(fis.GetRawObject(), FileAccess.Read))
                                        {
                                            byte[] data = new byte[stream.Length];
                                            stream.Read(data, 0, (int)stream.Length);
                                            
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Failed to open intent file descriptor");
                                }
                            }
                        }
                        */
                    }
                    else
                    {
                        Debug.LogError("[Intent] intentFile is null");
                    }
                }
                else
                {
                    Debug.LogError("[Intet] Failed to get intent");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        Debug.LogWarning($"[Intent] intentFile? {intentFile}");

        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
