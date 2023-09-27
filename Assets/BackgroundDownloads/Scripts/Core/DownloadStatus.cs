using System;

/// <summary>
/// Defines the states a background download may be in.
/// Not all values are applicable for all platforms.
/// </summary>
public enum DownloadStatus : int
{
    /// <summary>
    /// The download is pending (was scheduled, but was not started yet).
    /// </summary>
	Pending = 1 << 0,

    /// <summary>
    /// The download is running (currently downloading).
    /// </summary>
	Running = 1 << 1,

    /// <summary>
    /// The download was paused (platform dependent, may not be supported on all platforms).
    /// </summary>
	Paused = 1 << 2,

    /// <summary>
    /// The download was completed successfully.
    /// </summary>
	Successful = 1 << 3,

    /// <summary>
    /// The download was completed with an error (failed).
    /// </summary>
	Failed = 1 << 4
}