using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using NRKernal;
using NRKernal.Record;
using System;

public class EditorVideoEncoder: MonoBehaviour
{
    public int width = 1920;
    public int height = 1080;
    public int frameRate = 30;
    public int frameCount = 0;
    Texture2D frame;
    
    private bool m_IsStarted = false;
    private IntPtr m_TexPtr = IntPtr.Zero;

    // The time (in seconds) between each frame capture
    float frameInterval;


    // The time (in seconds) since the last frame capture
    float elapsedTime;

    string inputFilePattern;
    string outputFilePath;

    public Camera captureCameraReference;

    RenderTexture _mRenderTexture;
    public NativeEncodeConfig EncodeConfig;

    public void RegisterRenderTexture(RenderTexture renderTexture)
    {
        _mRenderTexture = renderTexture;
    }

    public void Config(CameraParameters param)
    {
        NRDebugger.Info("[EditorVideoEncoder] Config " + param.ToString());
        EncodeConfig = new NativeEncodeConfig(param);
        // androidMediaProjection = (param.mediaProjection != null) ? param.mediaProjection.GetRawObject() : IntPtr.Zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_IsStarted) {
            return;
        }

        if(EncodeConfig == null)
        {
            NRDebugger.Info("[VideoEncoder] EncodeConfig not set. call Config()");
            return;
        }

        inputFilePattern = Path.Combine(Application.persistentDataPath, "frame%d.png");
        outputFilePath = Path.Combine(Application.persistentDataPath, "output.mp4");

        NRDebugger.Info("[VideoEncoder] Start");
        NRDebugger.Info("[VideoEncoder] Config {0}", EncodeConfig.ToString());

        // Create a new Texture2D to hold the frame
        Texture2D frame = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        // Calculate the time interval between each frame capture
        frameInterval = 1f / frameRate;

        frameCount = 0;

        // Create the render texture
        //_mRenderTexture = new RenderTexture(width, height, 24);

        // Set the render texture as the active render texture
        //RenderTexture.active = _mRenderTexture;

        //captureCameraReference.targetTexture = _mRenderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        if(_mRenderTexture == null)
        {
            //UnityEngine.Debug.Log("render texture not registered");
            return;
        }

        // Increment the elapsed time
        elapsedTime += Time.deltaTime;

        // If the elapsed time is greater than or equal to the frame interval, capture a frame
        // capping at specific fps
        if (elapsedTime >= frameInterval)
        {
            // Read the pixels from the active render texture and save them to the Texture2D
            RenderTexture prevRenderTexture = RenderTexture.active;
            RenderTexture.active = _mRenderTexture;
            frame.ReadPixels(new Rect(0, 0, _mRenderTexture.width, _mRenderTexture.height), 0, 0);
            frame.Apply();
            RenderTexture.active = prevRenderTexture;
            
            // Capture a frame
            SaveFrame(frame);

            // Reset the elapsed time
            elapsedTime = 0;
            frameCount++;
        }
    }

    void SaveFrame(Texture2D frame)
    {
        // Save the frame to a temporary file
        byte[] bytes = frame.EncodeToPNG();
        string filePath = Path.Combine(Application.temporaryCachePath, "frame" + Time.frameCount + ".png");
        File.WriteAllBytes(filePath, bytes);
    }

    /// <summary> Commits. </summary>
    /// <param name="rt">        The renderTexture.</param>
    /// <param name="timestamp"> The timestamp.</param>
    /*public void Commit(RenderTexture rt, UInt64 timestamp)
    {
        if (!m_IsStarted)
        {
            return;
        }
        if (m_TexPtr == IntPtr.Zero)
        {
            m_TexPtr = rt.GetNativeTexturePtr();
        }

#if !UNITY_EDITOR
            mNativeEncoder.UpdateSurface(m_TexPtr, timestamp);

            if (m_AudioEncodeTool != null)
            {
                bool result = m_AudioEncodeTool.Flush(ref m_AudioRawData);
                if (result)
                {
                    mNativeEncoder.UpdateAudioData(m_AudioRawData, m_AudioEncodeTool.SampleRate,2,1);
                }
            }
#endif
    }*/

    void SaveVideo()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "ffmpeg";
        startInfo.Arguments = $"-framerate {frameRate} -i {inputFilePattern} -c:v libx264 -r 30 -pix_fmt yuv420p {outputFilePath}";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        Process.Start(startInfo);
    }
}