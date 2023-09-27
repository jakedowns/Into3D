using NUnit.Framework;
using System;
using System.IO;

namespace BackgroundDownload.Tests
{
    [TestFixture]
    public class BackgroundDownloadsTests
    {
        [Test]
        public void CancelDownload_CalledWithNull_DoesntThrow()
        {
            BackgroundDownloads.CancelDownload(null);
        }

        [Test]
        public void StartDownload_CalledWithNull_ThrowsArgumentNullException()
        {
            string url = null;

            Assert.Throws<ArgumentException>(() =>
            {
                BackgroundDownloads.StartDownload(url);
            });
        }

        [Test]
        public void StartDownload_CalledWithStringEmpty_DoesntThrow()
        {
            string url = string.Empty;

            Assert.Throws<ArgumentException>(() =>
            {
                BackgroundDownloads.StartDownload(url);
            });
        }

        [Test]
        public void StartDownload_CalledWithUrl_ReturnsOperation()
        {
            string url = @"https://www.google.com";
            var op = BackgroundDownloads.StartDownload(url);

            Assert.That(op.ID != -1, "DownloadOperation.ID should not be -1!");
        }

        [Test]
        public void StartDownload_CalledWithOptions_ReturnsOperation()
        {
            string url = @"https://www.google.com";

            var options = new BackgroundDownloadOptions(url);
            var op = BackgroundDownloads.StartDownload(options);

            Assert.That(op.ID != -1, "DownloadOperation.ID should not be -1!");
        }

        [Test]
        public void StartDownload_CalledWithOptions_CorrectDestinationPath()
        {
            string url = @"https://www.google.com/test.bundle";

            var options = new BackgroundDownloadOptions(url);
            var op = BackgroundDownloads.StartDownload(options);

            Assert.AreEqual(Path.Combine(BackgroundDownloadOptions.DEFAULT_DOWNLOAD_PATH, "test.bundle"), op.DestinationPath);
        }

        [Test]
        public void StartDownload_OptionsWithCustomDestinationPath_CorrectDestinationPath()
        {
            string url = @"https://www.google.com/test.bundle";

            var destinationPath = Path.Combine("D:", "MyPath");

            var options = new BackgroundDownloadOptions(url).SetDestinationPath(destinationPath);
            var op = BackgroundDownloads.StartDownload(options);

            var expectedPath = Path.Combine(destinationPath, "test.bundle");

            Assert.AreEqual(expectedPath, op.DestinationPath);
        }
    }
}