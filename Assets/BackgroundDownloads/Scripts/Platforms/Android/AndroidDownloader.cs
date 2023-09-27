#if UNITY_ANDROID

using System;
using UnityEngine;
using UnityEngine.Profiling;

public class AndroidDownloader : IBackgroundDownloader
{
	private const string JAVA_CLASS_NAME = "com.xorhead.tools.DownloadService";

	// Properties

	private static AndroidJavaClass androidBridge;

	private static AndroidJavaClass AndroidBridge
	{
		get
		{
			if (androidBridge == null)
			{
				androidBridge = new AndroidJavaClass(JAVA_CLASS_NAME);
			}

			return androidBridge;
		}
	}

	// Methods

	private static AndroidJavaObject context;

	/// <summary>
	/// Helper method for retrieving the context (main activity) of the app.
	/// </summary>
	private static AndroidJavaObject Context
	{
		get
		{
			if (context == null)
			{
				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}

			return context;
		}
	}

	public DownloadOperation StartDownload(BackgroundDownloadOptions options)
	{
		try
		{
			var id = AndroidBridge.CallStatic<long>("download", Context, options.URL, options.DestinationPath, options.Title, options.Description, (int)options.DownloadNotificationVisibility);

			return new AndroidDownloadOperation(this, options, id);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}

		return null;
	}

	public DownloadOperation StartDownload(string url)
	{
		BackgroundDownloadOptions options = new BackgroundDownloadOptions(url);

		return StartDownload(options);
	}

    public void CancelDownload(DownloadOperation operation)
	{
		Profiler.BeginSample(string.Format("CancelDownload {0}", operation.ID));

		AndroidBridge.CallStatic("cancelDownload", Context, operation.ID);

		Profiler.EndSample();
	}

	public DownloadStatus GetStatus(long id)
	{
		return (DownloadStatus)AndroidBridge.CallStatic<int>("getDownloadStatus", Context, id);
	}

	public float GetProgress(long id)
	{
		return AndroidBridge.CallStatic<float>("getDownloadProgress", Context, id);
	}

	public DownloadOperation GetDownloadOperation (string url)
	{
		var id = AndroidBridge.CallStatic<long> ("getDownloadOperation", Context, url);

		return (id == -1) ? null : new AndroidDownloadOperation (this, new BackgroundDownloadOptions (url), id);
	}
}

#endif