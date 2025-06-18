using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;

namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for ImageUploaderServiceTest and is intended
///to contain all ImageUploaderServiceTest Unit Tests
///</summary>
[TestClass()]
public class ImageServiceTest
{
    private static AlbumDetail? _albumTest = null;
    private TestContext? testContextInstance;
    private static int _i = 0;
    private int _iteration = 0;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext? TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }

    /// <summary>
    /// Setup this class and create a test album to work with
    /// </summary>
    /// <param name="testContext"></param>
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
    }

    /// <summary>
    /// Setup this class and create a test album to work with
    /// </summary>
    [TestInitialize()]
    public void MyTestInitialize()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

        _iteration = _i++;

        var core = Utility.RetrieveSmugMugCore();
        var createAlbumTask = Utility.CreateArbitraryTestAlbum(core, $"TestAlbum{_iteration.ToString()}");
        createAlbumTask.Wait();
        _albumTest = createAlbumTask.Result;
    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [TestCleanup()]
    public void MyTestCleanup()
    {
        var core = Utility.RetrieveSmugMugCore();
        var cleanupAlbumTask = Utility.RemoveArbitraryTestAlbum(core, "TestAlbum{_iteration.ToString()}");
        cleanupAlbumTask.Wait();
        _ = cleanupAlbumTask.Result;
    }

    /// <summary>
    /// Add support for adding a default test image when tests require it
    /// </summary>
    /// <param name="core"></param>
    /// <returns></returns>
    private async Task<ImageUpload> AddTestImage(SmugMugCore.Net.Core.SmugMugCore core)
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var target = new ImageUploaderService(core); 
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);

        return await target.UploadNewImage(albumId, content);
    }

    /// <summary>
    ///A test for Delete
    ///</summary>
    [TestMethod()]
    public async Task DeleteTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageService(core);
        var imageTest = await AddTestImage(core);
        long imageId = imageTest.ImageId; 
        int albumId = _albumTest.AlbumId; 
        bool expected = true; 
        bool actual;
        actual = await target.Delete(imageId, albumId);

        var albumCheck = new AlbumService(core);
        var albumData = await albumCheck.GetAlbumDetail(_albumTest.AlbumId, _albumTest.AlbumKey);
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(0, albumData.ImageCount);
    }

    /// <summary>
    ///A test for GetAlbumImages
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumImagesTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        int albumId = _albumTest.AlbumId;
        string albumKey = _albumTest.AlbumKey;
        string albumPassword = string.Empty; 
        string sitePassword = string.Empty; 
        bool loadImageInfo = false;
        AlbumDetail actual = await target.GetAlbumImagesExt([], albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

        if (actual.Images == null || actual.ImageCount == 0)
            Assert.Fail("No Images were loaded.");

        // Verify there is one image and it is the test image
        Assert.AreEqual(1, actual.ImageCount);
        Assert.AreEqual(imageTest.ImageId, actual.Images[0].ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.Images[0].ImageKey);

    }

    /// <summary>
    ///A test for GetAlbumImages
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumImagesExtraTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        int albumId = _albumTest.AlbumId;
        string albumKey = _albumTest.AlbumKey;
        string albumPassword = string.Empty;
        string sitePassword = string.Empty;
        bool loadImageInfo = false;
        AlbumDetail actual = await target.GetAlbumImagesExt(["Keywords"], albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

        if (actual.Images == null || actual.ImageCount == 0)
            Assert.Fail("No Images were loaded.");

        // Verify there is one image and it is the test image
        Assert.AreEqual(1, actual.ImageCount);
        Assert.AreEqual(imageTest.ImageId, actual.Images[0].ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.Images[0].ImageKey);

    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    public async Task GetImageInfoTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        string imageKey = imageTest.ImageKey; 
        string albumPassword = string.Empty; 
        string sitePassword = string.Empty; 
        bool includeOnlyUrls = false; 
        var actual = await target.GetImageInfoExt(imageId, imageKey, "", albumPassword, sitePassword, includeOnlyUrls);
        Assert.AreEqual(imageTest.ImageId, actual.ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.ImageKey);
        Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", imageTest.ImageKey)), "UrlLightboxURL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1280URL, "UrlVideo1280URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1920URL, "UrlVideo1920URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo320URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo640URL, "UrlVideo640URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo960URL, "UrlVideo960URL");
        Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", imageTest.ImageKey)), "UrlViewOriginalURL");
        Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("/L/i-{0}-L.jpg", imageTest.ImageKey)), "UrlViewLargeURL");
        Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("/M/i-{0}-M.jpg", imageTest.ImageKey)), "UrlViewMediumURL");
        Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("/S/i-{0}-S.jpg", imageTest.ImageKey)), "UrlViewSmallURL");
        Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("/Th/i-{0}-Th.jpg", imageTest.ImageKey)), "UrlViewThumbURL");
        Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("/Ti/i-{0}-Ti.jpg", imageTest.ImageKey)), "UrlViewTinyURL");
        Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("/X2/i-{0}-X2.jpg", imageTest.ImageKey)), "UrlViewX2LargeURL");
        Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("/X3/i-{0}-X3.jpg", imageTest.ImageKey)), "UrlViewX3LargeURL");
        Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("/XL/i-{0}-XL.jpg", imageTest.ImageKey)), "UrlViewXLargeURL");
    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    public async Task GetImageInfoUrlsOnlyTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId;
        string imageKey = imageTest.ImageKey;
        string albumPassword = string.Empty;
        string sitePassword = string.Empty;
        bool includeOnlyUrls = true;
        var actual = await target.GetImageInfoExt(imageId, imageKey, "", albumPassword, sitePassword, includeOnlyUrls);
        Assert.AreEqual(imageTest.ImageId, actual.ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.ImageKey);
        Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", imageTest.ImageKey)), "UrlLightboxURL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1280URL, "UrlVideo1280URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1920URL, "UrlVideo1920URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo320URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo640URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo960URL, "UrlVideo320URL");
    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    public async Task GetImageInfoUrlsCustomSizeTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId;
        string imageKey = imageTest.ImageKey;
        string albumPassword = string.Empty;
        string sitePassword = string.Empty;
        bool includeOnlyUrls = true;
        var actual = await target.GetImageInfoExt(imageId, imageKey, "100", albumPassword, sitePassword, includeOnlyUrls);
        Assert.AreEqual(imageTest.ImageId, actual.ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.ImageKey);
        Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", imageTest.ImageKey)));
        Assert.AreEqual(string.Empty, actual.UrlVideo1280URL, "UrlVideo1280URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1920URL, "UrlVideo1920URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo320URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo640URL, "UrlVideo640URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo960URL, "UrlVideo960URL");
    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    public async Task GetVideoInfoTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageService(core);

        var uploader = new ImageUploaderService(core);
        var filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestVideo.mov");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        var videoTest = await uploader.UploadNewImage(_albumTest.AlbumId, content);

        long videoId = videoTest.ImageId;
        string videoKey = videoTest.ImageKey;
        var actual = await target.GetImageInfoExt(videoId, videoKey, "");
        Assert.AreEqual(videoTest.ImageId, actual.ImageId);
        Assert.AreEqual(videoTest.ImageKey, actual.ImageKey);
    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    public async Task GetVideoInfoPropertiesTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageService(core);

        var uploader = new ImageUploaderService(core);
        var filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestVideo.mov");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        var videoTest = await uploader.UploadNewImage(_albumTest.AlbumId, content);

        long videoId = videoTest.ImageId;
        string videoKey = videoTest.ImageKey;

        var actual = await target.GetImageInfo(videoId, videoKey);

        Assert.AreEqual(videoTest.ImageId, actual.ImageId);
        Assert.AreEqual(videoTest.ImageKey, actual.ImageKey);
        if (actual.Album != null)
        {
            Assert.AreEqual(_albumTest.AlbumId, actual.Album.AlbumId);
            Assert.AreEqual(_albumTest.AlbumKey, actual.Album.AlbumKey);
        }
        else
            Assert.Fail("Album was not part of the Image Structure");
        Assert.AreEqual("This is a test Video", actual.Caption);
        if (actual.DateUploaded != null)
        {
            var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
            Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
        }
        else
            Assert.Fail("Image Uploaded Date was not provided, or is null");
        Assert.AreEqual("TestVideo.mov", actual.Filename);
        Assert.AreEqual(1920, actual.Width);
        Assert.AreEqual(false, actual.Hidden);
        Assert.AreEqual("Scenic; Animals; Activity; Cruise", actual.Keywords);
        if (actual.LastUpdatedDate != null)
        {
            var tsUpdated = DateTime.Now - DateTime.Parse(actual.LastUpdatedDate);
            Assert.IsTrue(tsUpdated.Days < 1, "Updated less than a day ago");
        }
        else
            Assert.Fail("Image Last Updated Date was not provided, or is null");
        Assert.IsNotNull(actual.MD5Sum);
        Assert.IsTrue(actual.SizeBytes > 100000);
        Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", videoTest.ImageKey)), "UrlLightboxURL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1280URL, "UrlVideo1280URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1920URL, "UrlVideo1920URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo320URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo640URL, "UrlVideo640URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo960URL, "UrlVideo960URL");
        Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", videoTest.ImageKey)), "UrlViewOriginalURL");
        Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("/L/i-{0}-L.jpg", videoTest.ImageKey)), "UrlViewLargeURL");
        Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("/M/i-{0}-M.jpg", videoTest.ImageKey)), "UrlViewMediumURL");
        Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("/S/i-{0}-S.jpg", videoTest.ImageKey)), "UrlViewSmallURL");
        Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("/Th/i-{0}-Th.jpg", videoTest.ImageKey)), "UrlViewThumbURL");
        Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("/Ti/i-{0}-Ti.jpg", videoTest.ImageKey)), "UrlViewTinyURL");
        Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("/X2/i-{0}-X2.jpg", videoTest.ImageKey)), "UrlViewX2LargeURL");
        Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("/X3/i-{0}-X3.jpg", videoTest.ImageKey)), "UrlViewX3LargeURL");
        Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("/XL/i-{0}-XL.jpg", videoTest.ImageKey)), "UrlViewXLargeURL");
    }

    /// <summary>
    ///A test for GetImageInfo
    ///</summary>
    [TestMethod()]
    public async Task GetImageInfoPropertiesTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId;
        string imageKey = imageTest.ImageKey;

        var actual = await target.GetImageInfo(imageId, imageKey);

        Assert.AreEqual(imageTest.ImageId, actual.ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.ImageKey);
        if (actual.Album != null)
        {
            Assert.AreEqual(_albumTest.AlbumId, actual.Album.AlbumId);
            Assert.AreEqual(_albumTest.AlbumKey, actual.Album.AlbumKey);
        }
        else
            Assert.Fail("Album was not part of the Image Structure");
        Assert.AreEqual("Pepper", actual.Caption, "Caption");
        if (actual.DateUploaded != null)
        {
            var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
            Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
        }
        else
            Assert.Fail("Image Uploaded Date was not provided, or is null");
        Assert.AreEqual("TestImage.jpg", actual.Filename, "Filename");
        Assert.AreEqual(640, actual.Width, "Width");
        Assert.AreEqual(false, actual.Hidden, "Hidden");
        Assert.AreEqual("Pets; Pepper; Activity; Snow", actual.Keywords, "Keywords");
        if (actual.LastUpdatedDate != null)
        {
            var tsUpdated = DateTime.Now - DateTime.Parse(actual.LastUpdatedDate);
            Assert.IsTrue(tsUpdated.Days < 1, "Updated less than a day ago");
        }
        else
            Assert.Fail("Image Last Updated Date was not provided, or is null");
        Assert.AreEqual("be8859c3de0d53c44d8d3d926e4637aa", actual.MD5Sum, "MD5Sum");
        Assert.AreEqual(184933, actual.SizeBytes, "SizeBytes");
        Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", imageTest.ImageKey)), "UrlLightboxURL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1280URL, "UrlVideo1280URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo1920URL, "UrlVideo1920URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo320URL, "UrlVideo320URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo640URL, "UrlVideo640URL");
        Assert.AreEqual(string.Empty, actual.UrlVideo960URL, "UrlVideo960URL");
        Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", imageTest.ImageKey)), "UrlViewOriginalURL");
        Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("/L/i-{0}-L.jpg", imageTest.ImageKey)), "UrlViewLargeURL");
        Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("/M/i-{0}-M.jpg", imageTest.ImageKey)), "UrlViewMediumURL");
        Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("/S/i-{0}-S.jpg", imageTest.ImageKey)), "UrlViewSmallURL");
        Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("/Th/i-{0}-Th.jpg", imageTest.ImageKey)), "UrlViewThumbURL");
        Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("/Ti/i-{0}-Ti.jpg", imageTest.ImageKey)), "UrlViewTinyURL");
        Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("/X2/i-{0}-X2.jpg", imageTest.ImageKey)), "UrlViewX2LargeURL");
        Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("/X3/i-{0}-X3.jpg", imageTest.ImageKey)), "UrlViewX3LargeURL");
        Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("/XL/i-{0}-XL.jpg", imageTest.ImageKey)), "UrlViewXLargeURL");
    }

    /// <summary>
    ///A test for UpdateImage
    ///</summary>
    [TestMethod()]
    public async Task UpdateImageTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);

        System.Threading.Thread.Sleep(20000);
        var origImage = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
        origImage.Caption = "New Caption";
        origImage.Filename = "NewFile.jpg";
        origImage.Hidden = true;
        origImage.Keywords = "A Keyword; AnotherKeyword";

        bool expected = true;
        bool actual = await target.UpdateImage(origImage);
        Assert.AreEqual(expected, actual);

        var updatedImage = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
        Assert.AreEqual(origImage.Caption, updatedImage.Caption, "Caption");
        Assert.AreEqual(origImage.Filename, updatedImage.Filename, "Filename");
        Assert.AreEqual(origImage.Hidden, updatedImage.Hidden, "Hidden");
        Assert.AreEqual(origImage.Keywords, updatedImage.Keywords, "Keywords");
    }

    /// <summary>
    ///A test for DownloadImage
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(System.Net.Http.HttpRequestException))]
    public async Task DownloadNoImageTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        var origImage = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);

        origImage.UrlViewOriginalURL = origImage.UrlViewOriginalURL.Replace(origImage.ImageKey, "AAA");
        string localPath = Path.GetTempFileName();
        var dlSuccess = await target.DownloadImage(origImage, localPath);

        var fi = new FileInfo(localPath);
        long actual = fi.Length;

        // Cleanup file
        fi.Delete();

        Assert.AreEqual(0, actual);
    }

    /// <summary>
    ///A test for DownloadImage
    ///</summary>
    [TestMethod()]
    public async Task DownloadImageTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        var origImage = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);

        string localPath = Path.GetTempFileName();
        var dlSuccess  = await target.DownloadImage(origImage, localPath);

        var fi = new FileInfo(localPath);
        long actual = fi.Length;

        // Cleanup file
        fi.Delete();
        
        Assert.AreNotEqual(0, actual);
    }
}
