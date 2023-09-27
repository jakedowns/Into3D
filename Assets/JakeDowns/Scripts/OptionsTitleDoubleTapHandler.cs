using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsTitleDoubleTapHandler : MonoBehaviour
{
    GameObject ScreenRecorder;
    // Start is called before the first frame update
    void Start()
    {
        ScreenRecorder = GameObject.Find("VideoCaptureExample");
        ScreenRecorder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSingleTap(string name)
    {
        Debug.Log($"[OptionsTitle] Single Tap Triggered {name}");
        if (name == "OptionsTitle")
        {
            
        }
    }

    // we require a double-tap to unlock
    public void OnDoubleTap(string name)
    {
        Debug.Log($"[OptionsTitle] Double Tap Triggered {name}");
        if (name == "OptionsTitle")
        {
            ScreenRecorder.SetActive(ScreenRecorder.activeSelf ? false : true);
        }
    }
}
