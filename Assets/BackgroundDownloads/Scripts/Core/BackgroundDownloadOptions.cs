using System.IO;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public enum DownloadNotificationVisibility
{
	/// <summary>
	/// This download doesn't show in the UI or in the notifications.
	/// </summary>
	VISIBILITY_HIDDEN = 2,
	
	/// <summary>
	/// This download is visible but only shows in the notifications while it's in progress.
	/// </summary>
	VISIBILITY_VISIBLE = 0,
	
	/// <summary>
	/// This download is visible and shows in the notifications while in progress and after completion.
	/// </summary>
	VISIBILITY_VISIBLE_NOTIFY_COMPLETED = 1,
	
	/// <summary>
	/// This download shows in the notifications after completion ONLY.
	/// </summary>
	VISIBILITY_VISIBLE_NOTIFY_ONLY_COMPLETION = 3
}

/// <summary>
/// Options for customizing the background download process.
/// </summary>
public class BackgroundDownloadOptions
{
	public static readonly string DEFAULT_DOWNLOAD_PATH = Application.persistentDataPath;

    private const string DEFAULT_DESCRIPTION = "Downloading...";

    /// <summary>
    /// The URL for the data that should be downloaded.
    /// </summary>
    public string URL { get; private set; }

    private string destinationPath;

    /// <summary>
    /// The path to save the downloaded data in, once the download completes.
    /// </summary>
	public string DestinationPath
	{
		get { return destinationPath ?? GetDefaultDestinationPath(); }
	}

	private string GetDefaultDestinationPath()
	{
		return Path.Combine(DEFAULT_DOWNLOAD_PATH, Path.GetFileName(URL));	
	}

	private string title;

    /// <summary>
    /// The title to display for the ongoing download in the notification area.
    /// </summary>
	public string Title
	{
		get { return title ?? GetDefaultTitle(); }
	}

	private string GetDefaultTitle()
	{
		return Application.productName;
	}

	private string description;
    
    /// <summary>
    /// The description to display for the ongoing download in the notification area.
    /// </summary>
    public string Description
	{
		get { return description ?? DEFAULT_DESCRIPTION; }
	}

    private DownloadNotificationVisibility downloadNotificationVisibility = DownloadNotificationVisibility.VISIBILITY_VISIBLE;

    /// <summary>
    /// Returns a value indicating the download notification visibility (in the notification area) when the app is in
    /// the background.
    /// NOTE: this setting is *ONLY* relevant for Android
    /// </summary>
    public DownloadNotificationVisibility DownloadNotificationVisibility
    {
	    get { return downloadNotificationVisibility;  }
    }
    
	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundDownloadOptions"/> class.
	/// </summary>
	public BackgroundDownloadOptions(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new System.ArgumentException("url should not be null or empty!");
		}

        System.Uri uri;

        // Verify that the passed string can be parsed as a URL
        if (!System.Uri.TryCreate(url, System.UriKind.Absolute, out uri))
        {
            throw new System.ArgumentException("url is not a valid URL address!");
        }

        this.URL = url;
	}

	/// <summary>
	/// Sets the destination path (folder) for storing the downloaded data.
	/// If this is not called, data is stored by the result of GetDefaultDestinationPath().
    /// NOTE: By default, data is stored under <see cref="Application.persistentDataPath"/>
	/// </summary>
	public BackgroundDownloadOptions SetDestinationPath(string destinationPath)
	{
		if (!string.IsNullOrEmpty(destinationPath))
		{
            this.destinationPath = Path.Combine(destinationPath, Path.GetFileName(URL));
		}

		return this;
	}

	/// <summary>
	/// Sets the notification title text that will be shown while the download is active.
	/// </summary>
	public BackgroundDownloadOptions SetTitle(string title)
	{
		if (title != null)
		{
			this.title = title;
		}

		return this;
	}

	/// <summary>
	/// Sets the notification description text that will be shown while the download is active.
    /// NOTE: This may not be applicable on every platform!
	/// </summary>
	public BackgroundDownloadOptions SetDescription(string description)
	{
		if (description != null)
		{
			this.description = description;
		}

		return this;	
	}
	
	/// <summary>
	/// Sets the notification description text that will be shown while the download is active.
	/// NOTE: This may not be applicable on every platform!
	/// </summary>
	public BackgroundDownloadOptions SetNotificationVisibility(DownloadNotificationVisibility notificationVisibility)
	{
		this.downloadNotificationVisibility = notificationVisibility;
		
		return this;	
	}
}