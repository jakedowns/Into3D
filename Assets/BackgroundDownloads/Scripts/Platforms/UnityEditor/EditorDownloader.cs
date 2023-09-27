#if UNITY_EDITOR || UNITY_STANDALONE

using System.Collections.Generic;

public class EditorDownloader : IBackgroundDownloader
{
    private readonly Dictionary<string, EditorDownloadOperation> operations = new Dictionary<string, EditorDownloadOperation>();

	public void CancelDownload(DownloadOperation operation)
	{
        var editorOperation = operation as EditorDownloadOperation;

        if (editorOperation == null)
        {
            return;
        }

        editorOperation.Cancel();
	}

	public DownloadOperation StartDownload(BackgroundDownloadOptions options)
	{
		return new EditorDownloadOperation(this, options);
	}

	public DownloadOperation StartDownload(string url)
	{
		var options = new BackgroundDownloadOptions(url);

		return StartDownload(options);
	}

	public DownloadOperation GetDownloadOperation (string url)
	{
        return operations.ContainsKey(url) ? operations[url] : null;
	}

    public void ClearDownload(string url)
    {
        operations.Remove(url);
    }
}

#endif