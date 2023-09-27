#if UNITY_ANDROID

using BackgroundDownload.Utils;

public class AndroidDownloadOperation : DownloadOperation
{
    private readonly Cached<DownloadStatus> cachedStatus;
    private readonly Cached<float> cachedProgress;

    public AndroidDownloadOperation(AndroidDownloader downloader, BackgroundDownloadOptions options, long id)
		: base(options)
	{
		this.id = id;

        cachedStatus = new Cached<DownloadStatus>(() => downloader.GetStatus(id));
        cachedProgress = new Cached<float>(() => downloader.GetProgress(id));
    }

    public override float Progress
	{
		get
		{
			return ID > BackgroundDownloads.DOWNLOAD_ERROR_ID && string.IsNullOrEmpty(Error) ? cachedProgress : 0f;
		}
	}

	public override DownloadStatus Status
	{
		get
		{
			return cachedStatus;
		}
	}
}

#endif