using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;

namespace BackgroundDownload.Tests
{
    [TestFixture]
    [Category("BackgroundDownload")]
    public class BackgroundDownloadOptionsTests
    {
        [Test]
        public void Ctor_UrlNull_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new BackgroundDownloadOptions(null);
            });
        }

        [Test]
        public void Ctor_UrlEmptyString_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new BackgroundDownloadOptions(string.Empty);
            });
        }

        [Test]
        public void Ctor_InvalidUrl_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new BackgroundDownloadOptions("abcd");
            });
        }

        [Test]
        public void Title_WithoutCallingSetTitle_ReturnsDefaultTitle()
        {
            var url = @"https://www.google.com/test";
            var options = new BackgroundDownloadOptions(url);

            Assert.AreEqual(Application.productName, options.Title);
        }

        [Test]
        public void Title_AfterCallingSetTitle_ReturnsSetTitle()
        {
            var url = @"https://www.google.com/test";

            var options = new BackgroundDownloadOptions(url).SetTitle("My Title");

            Assert.AreEqual("My Title", options.Title);
        }

        [Test]
        public void DestinationPath_AfterSettingToNull_ReturnsDefaultPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";

            var options = new BackgroundDownloadOptions(url).SetDestinationPath(null);

            Assert.AreEqual(Path.Combine(BackgroundDownloadOptions.DEFAULT_DOWNLOAD_PATH, "myTestAssetBundle.bin"), options.DestinationPath);
        }

        [Test]
        public void DestinationPath_AfterSettingToEmpty_ReturnsDefaultPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";

            var options = new BackgroundDownloadOptions(url).SetDestinationPath(string.Empty);

            Assert.AreEqual(Path.Combine(BackgroundDownloadOptions.DEFAULT_DOWNLOAD_PATH, "myTestAssetBundle.bin"), options.DestinationPath);
        }

        [Test]
        public void DestinationPath_AfterSettingToNull_ReturnsPreviouslySetPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";
            var destinationPath = Path.Combine("D:", "MyCache");

            var options = new BackgroundDownloadOptions(url).SetDestinationPath(destinationPath).SetDestinationPath(null);

            var expectedPath = Path.Combine(destinationPath, "myTestAssetBundle.bin");

            Assert.AreEqual(expectedPath, options.DestinationPath);
        }

        [Test]
        public void DestinationPath_AfterSettingToEmpty_ReturnsPreviouslySetPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";
            var destinationPath = Path.Combine("D:", "MyCache");

            var options = new BackgroundDownloadOptions(url).SetDestinationPath(destinationPath).SetDestinationPath(string.Empty);

            var expectedPath = Path.Combine(destinationPath, "myTestAssetBundle.bin");

            Assert.AreEqual(expectedPath, options.DestinationPath);
        }

        [Test]
        public void DestinationPath_WithoutCallingSetDestination_ReturnsDefaultPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";

            var options = new BackgroundDownloadOptions(url);

            Assert.AreEqual(Path.Combine(BackgroundDownloadOptions.DEFAULT_DOWNLOAD_PATH, "myTestAssetBundle.bin"), options.DestinationPath);
        }

        [Test]
        public void DestinationPath_AfterCallingSetDestination_ReturnsCorrectPath()
        {
            var url = @"https://www.google.com/myTestAssetBundle.bin";

            var destinationPath = Path.Combine("D:", "MyCache");
            var options = new BackgroundDownloadOptions(url).SetDestinationPath(destinationPath);

            var expectedPath = Path.Combine(destinationPath, "myTestAssetBundle.bin");
            
            Assert.AreEqual(expectedPath, options.DestinationPath);
        }

        [Test]
        public void URL_AfterConstructingWithURL_ReturnsCorrectURL()
        {
            var url = @"https://www.google.com/test";

            var options = new BackgroundDownloadOptions(url);

            Assert.AreEqual(url, options.URL);
        }
    }
}