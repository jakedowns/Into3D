using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// Provides an API for downloading files & data in the background (even when the app is not running).
/// The downloads can continue even when the game is minimized or closed.
/// </summary>
public static class BackgroundDownloads
{
    // Consts

    /// <summary>
    /// Indicates an error occurred while starting the download.
    /// </summary>
	internal const long DOWNLOAD_ERROR_ID = -1;

	// Members

	private static IBackgroundDownloader downloader;

    /// <summary>
    /// Returns the platform-specific <see cref="IBackgroundDownloader"/> implementation for the current platform.
    /// This instance is only initialized once; subsequent calls will return the cached reference of this instance.
    /// </summary>
    private static IBackgroundDownloader Downloader
    {
        get
        {
            if (downloader == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                downloader = new EditorDownloader();
#elif UNITY_ANDROID
                downloader = new AndroidDownloader();
#elif UNITY_IOS
                downloader = new iOSDownloader ();
#else
                Debug.LogError("Unsupported platform!");
#endif
            }

            return downloader;
        }
    }

    /// <summary>
    /// Starts a download with the given URL.
    /// Once the download completes it will be saved under the default path.
    /// <see cref="BackgroundDownloadOptions.DEFAULT_DOWNLOAD_PATH"/>
    /// </summary>
    /// <param name="url">The URL for the data that will be downloaded.</param>
    public static DownloadOperation StartDownload(string url)
	{
		BackgroundDownloadOptions options = new BackgroundDownloadOptions(url);

		return StartDownload(options);
	}

	/// <summary>
	/// Starts a download with the given download configuration options.
	/// </summary>
	public static DownloadOperation StartDownload(BackgroundDownloadOptions options)
	{
		Profiler.BeginSample(string.Format("StartDownload (options): {0}", options.URL));
		
		var op = Downloader.StartDownload(options);

		Profiler.EndSample();

		return op;
	}

    /// <summary>
    /// Starts or continues tracking an ongoing download with the given URL.
    /// </summary>
    /// <param name="url">The URL to download or track</param>
    /// <returns>A <see cref="DownloadOperation"/> instance that tracks the download of the given URL</returns>
    public static DownloadOperation StartOrContinueDownload(string url)
    {
        return StartOrContinueDownload(new BackgroundDownloadOptions(url));
    }

    /// <summary>
    /// Starts or continues tracking an ongoing download.
    /// If the download is not running yet, it will be started with the given download configuration options.
    /// </summary>
    /// <returns>A <see cref="DownloadOperation"/> instance that tracks the download of the given URL</returns>
    public static DownloadOperation StartOrContinueDownload(BackgroundDownloadOptions options)
    {
        var op = GetDownloadOperation(options.URL);

        if (op == null || op.ID == DOWNLOAD_ERROR_ID)
        {
            Debug.LogError("no download was found for: " + options.URL);

            op = StartDownload(options);
        }

        return op;
    }
    
	/// <summary>
	/// Cancels the download that is tracked by the given <see cref="DownloadOperation"/> object.
	/// </summary>
	public static void CancelDownload(DownloadOperation operation)
	{
        if (operation == null)
        {
            return;
        }

		Profiler.BeginSample(string.Format("CancelDownload {0}", operation.ID));

		Downloader.CancelDownload(operation);

		Profiler.EndSample();
	}

    /// <summary>
    /// Returns the ongoing <see cref="DownloadOperation"/> object associated with the given URL.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>The <see cref="DownloadOperation"/> object for the ongoing operation, or null if not download was found</returns>
	public static DownloadOperation GetDownloadOperation(string url)
	{
		return Downloader.GetDownloadOperation (url);
	}
}