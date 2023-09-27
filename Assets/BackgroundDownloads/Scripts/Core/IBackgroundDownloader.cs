using System;

/// <summary>
/// An interface for types that provide background downloading.
/// </summary>
public interface IBackgroundDownloader
{
	/// <summary>
	/// Starts a download of the given URL.
	/// </summary>
	DownloadOperation StartDownload(string url);

	/// <summary>
	/// Starts a download with the given options.
	/// </summary>
	DownloadOperation StartDownload(BackgroundDownloadOptions options);

	/// <summary>
	/// Cancels the given download operation.
	/// </summary>
	void CancelDownload(DownloadOperation operation);

	/// <summary>
	/// Searches for an ongoing download operation of the given URL.
	/// This method is used to retrieve a <see cref="DownloadOperation"/> object for a given URL.
    /// Example use case: After restarting the game, obtain a DownloadOperation object for an ongoing download
	/// that was previously started (in another session of the game) to continue displaying its progress.
	/// </summary>
	DownloadOperation GetDownloadOperation(string url);
}