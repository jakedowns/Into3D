
# 'Background Download' Documentation #

Background Download allows mobile apps and games to download data, even when the app is not running (e.g: minimized).
This is especially important for apps that download large amounts of data at runtime (e.g: downloadable content, such as asset bundles).


#### Usage Instructions

Usage is kept super simple (on purpose!). The main class that provides the APIs for downloading data is BackgroundDownloads.

1. To download a new file (in the background), call the StartDownload(url) method:

BackgroundDownloads.StartDownload(@"https://speed.hetzner.de/1GB.bin");

This will start a download of the file in the given URL. This download will continue to run, even when the app is minimized (sent to the background).


2. To cancel an ongoing download (that was previously started):

When starting a download (using the StartDownload method), a DownloadOperation object is returned.
This object is used to keep track of the ongoing download operation, and can provide other details about the download (status, progress, etc).

In order to cancel an ongoing download, call the CancelDownload method, passing in the DownloadOperation object related to that download:

// download was started
var op = BackgroundDownloads.StartDownload(@"https://speed.hetzner.de/1GB.bin");

BackgroundDownloads.CancelDownload(op);


3. To retrieve a previuosly started download (even from a previous game session):

Since downloads persist even while the game is sent to the background or killed, a download might already be in progress when you run your app/game again.
As such, it might be convenient to be able to query whether a download of a given URL is already in progress and continue tracking it (instead of starting a new download).

To achieve this, use the StartOrContinueDownload method:

var op = BackgroundDownloads.StartOrContinueDownload(@"https://speed.hetzner.de/1GB.bin");

This will either return the previuosly started operation, or create a new download operation.



#### Known Limitations with current release

- What platforms are supported?

Currently only Android, iOS and the Unity editor are supported.
While running in the editor, the WWW class is being used and so no "real" background download is performed (unless the editor is set to run the game while in the background).

- Are there any limitations with getting information about ongoing downloads?

On iOS, initialization might take some take (few seconds) so it is not advised to check for ongoing downloads right away as the game is started.