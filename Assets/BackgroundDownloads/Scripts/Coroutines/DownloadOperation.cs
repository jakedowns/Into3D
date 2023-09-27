using UnityEngine;

/// <summary>
/// Represents an ongoing (Background) download operation.
/// This class implements <see cref="CustomYieldInstruction"/> to allow an easy migration from
/// other Unity networking APIs, such as <see cref="WWW"/> or <see cref="UnityEngine.Networking.UnityWebRequest"/>
/// </summary>
public abstract class DownloadOperation : CustomYieldInstruction
{
    private const string ERROR_MESSAGE = "Download Failed";

    #region implemented abstract members of CustomYieldInstruction

    /// <summary>
    /// An internally ID assigned to this download operation.
    /// This ID may be (internally) used for querying or cancelling the ongoing download operation.
    /// </summary>
    protected long id;

	/// <summary>
	/// The ID assigned to this download operation.
	/// </summary>
	public long ID
	{
		get { return id; }
	}

    /// <summary>
    /// The full path for storing this download in (including the file name).
    /// </summary>
    public string DestinationPath
    {
        get { return options.DestinationPath; }
    }

    /// <summary>
    /// The current status of this download operation.
    /// </summary>
    public abstract DownloadStatus Status { get; }

    /// <summary>
    /// The current progress of this download operation (returned between 0 to 1).
    /// </summary>
	public abstract float Progress { get; }
	
    /// <summary>
    /// The configuration object with options that were used to start this download operation.
    /// </summary>
	protected BackgroundDownloadOptions options;

	/// <summary>
	/// Initializes a new instance of the <see cref="DownloadOperation"/> class.
	/// </summary>
	/// <param name="url">URL.</param>
	public DownloadOperation(string url)
		: this(new BackgroundDownloadOptions(url))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DownloadOperation"/> class.
	/// </summary>
	/// <param name="options">Options.</param>
	public DownloadOperation(BackgroundDownloadOptions options)
	{
		this.options = options;
	}

	public override bool keepWaiting
	{
		get
		{
			return !IsDone;
		}
	}

    /// <summary>
    /// Returns a boolean flag indicating whether the download operation is done (succesfully or not).
    /// </summary>
	public virtual bool IsDone
	{
		get
		{
			// cache the status, in case it performs any expensive operation, such as a native call (it is used twice below).
			var status = Status;

			return this.id > BackgroundDownloads.DOWNLOAD_ERROR_ID && (status == DownloadStatus.Successful || status == DownloadStatus.Failed);
		}
	}

    /// <summary>
    /// Returns an error in case the download failed, or null otherwise.
    /// </summary>
	public virtual string Error
	{
		get
		{
			return Status == DownloadStatus.Failed ? ERROR_MESSAGE : null;
		}
	}

    /// <summary>
    /// Allows implicit conversion of a download operation to its ID (long).
    /// </summary>
    public static implicit operator long(DownloadOperation op)
    {
        return op.id;
    }

	#endregion
}