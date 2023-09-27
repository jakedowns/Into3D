#if UNITY_EDITOR || UNITY_STANDALONE

using System.IO;
using UnityEngine;

public class EditorDownloadOperation : DownloadOperation
{
    private readonly EditorDownloader downloader;
	private WWW request;
    private bool isDone;

    /// <summary>
    /// A boolean flag indicating whether this download operation was cancelled.
    /// </summary>
    private bool isCancelled;

	public EditorDownloadOperation(EditorDownloader downloader, BackgroundDownloadOptions options)
			: base(options)
	{
        this.downloader = downloader;
		request = new WWW(options.URL);
	}

	public override bool IsDone
	{
		get
		{
            if (isDone)
            {
                return isDone;
            }

            var isRequestDone = request.isDone;
            var requestError = request.error;

            isDone = isCancelled || isRequestDone || !string.IsNullOrEmpty(requestError);

            if (isDone)
            {
                // Store data from request and dispose it
                if (isRequestDone && string.IsNullOrEmpty(requestError))
                {
                    var data = request.bytes;

                    File.WriteAllBytes(options.DestinationPath, data);
                }

                // Remove this download
                downloader.ClearDownload(request.url);
            }

            return isDone;
		}
	}

	public override bool keepWaiting
	{
		get
		{
			return !IsDone;
		}
	}

	public override float Progress
	{
		get
		{
			return string.IsNullOrEmpty(request.error) ? request.progress : 0f;
		}
	}

	public override string Error
	{
		get
		{
			return isCancelled ? string.Empty : request.error;
		}
	}

	public override DownloadStatus Status
	{
		get
		{
            if (isCancelled)
            {
                return DownloadStatus.Failed;
            }
			if (!string.IsNullOrEmpty(request.error))
			{
				return DownloadStatus.Failed;
			}
			else if (request.isDone)
			{
				return DownloadStatus.Successful;
			}
			else
			{
				return DownloadStatus.Running;
			}
		}
	}

    public void Cancel()
    {
        if (request != null)
        {
            if (isCancelled)
            {
                return;
            }

            request.Dispose();

            isCancelled = true;
        }
    }
}

#endif