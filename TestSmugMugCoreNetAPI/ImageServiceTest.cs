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
    private async Task<ImageUpload> AddTestImage(SmugMug.Net.Core.SmugMugCore core)
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
    ///A test for AddComment
    ///</summary>
    [TestMethod()]
    public async Task AddCommentTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageService(core);
        var imageTest = await AddTestImage(core);
        long imageId = imageTest.ImageId; 
        string imageKey = imageTest.ImageKey; 
        string comment = "This is a test image comment"; 
        int rating = 1; 
        Comment actual = await target.AddComment(Array.Empty<string>(), imageId, imageKey, comment, rating);
        Assert.IsTrue(actual.CommentId > 0);
    }

    /// <summary>
    ///A test for ChangePosition
    ///</summary>
    [TestMethod()]
    public async Task ChangePositionTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageService(core);
        var imageTest = await AddTestImage(core);

        // Upload a second image with a different caption
        var uploader = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
        ImageContent content = await core.ContentMetadataService.DiscoverMetadata(filename);
        content.Caption = "Alternate Photo";
        ImageUpload newImage = await uploader.UploadUpdatedImage(albumId, 0, content);

        // Check that it is the second image
        var infoBefore = await target.GetImageInfo(newImage.ImageId, newImage.ImageKey);
        Assert.AreEqual(2, infoBefore.PositionInAlbum);

        // Change the position
        int position = 1; 
        bool expected = true; 
        bool actual;
        actual = await target.ChangePosition(newImage.ImageId, position);
        Assert.AreEqual(expected, actual);

        var infoAfter = await target.GetImageInfo(newImage.ImageId, newImage.ImageKey);
        Assert.AreEqual(1, infoAfter.PositionInAlbum);

    }

    /// <summary>
    /// A test for Crop
    ///</summary>
    [TestMethod()]
    public async Task CropTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        int height = 5;
        int width = 5;
        int x = 5; 
        int y = 5; 
        bool expected = true; 
        bool actual;
        // Wait 5 seconds for this to persist into the library after it starts
        System.Threading.Thread.Sleep(10000);
        actual = await target.Crop(imageId, height, width, x, y);
        Assert.AreEqual(expected, actual);

        // TODO: SmugMug uploads the image, and says it cropped it but didn't because image was still processing.
        int count = 0;
        ImageDetail? imageAfter = null;
        do
        {
            var img = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
            if (img.Height == 5)
            {
                imageAfter = img;
                count = 10;
            }
            else
            {
                count++;
                System.Threading.Thread.Sleep(1000);
            }

        } while (count < 10);
        Assert.IsNotNull(imageAfter);
        Assert.AreEqual(5, imageAfter.Height);
        Assert.AreEqual(5, imageAfter.Width);

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
        AlbumDetail actual = await target.GetAlbumImagesExt(Array.Empty<string>(), albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

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
        AlbumDetail actual = await target.GetAlbumImagesExt(new string[] { "Keywords" }, albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

        if (actual.Images == null || actual.ImageCount == 0)
            Assert.Fail("No Images were loaded.");

        // Verify there is one image and it is the test image
        Assert.AreEqual(1, actual.ImageCount);
        Assert.AreEqual(imageTest.ImageId, actual.Images[0].ImageId);
        Assert.AreEqual(imageTest.ImageKey, actual.Images[0].ImageKey);

    }

    /// <summary>
    ///A test for GetCommentList
    ///</summary>
    [TestMethod()]
    public async Task GetCommentListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        string imageKey = imageTest.ImageKey;
        string albumPassword = string.Empty; 
        string sitePassword = string.Empty; 

        // Add a comment
        string text = "This is a test image comment"; 
        int rating = 1; 
        Comment comment = await target.AddComment(Array.Empty<string>(), imageId, imageKey, text, rating);

        var actual = await target.GetCommentList(imageId, imageKey, albumPassword, sitePassword);
        var commentTestSearch = actual.Where(x => x.CommentId == comment.CommentId);
        
        Assert.AreEqual(commentTestSearch.Count(), 1);
        var commentTest = commentTestSearch.Single();

        Assert.AreEqual(comment.CommentId, commentTest.CommentId);
        Assert.AreEqual(rating, commentTest.Rating);
        Assert.AreEqual(text, commentTest.Text);
        Assert.IsNotNull(commentTest.DatePosted, "Comment date should not be null.");
        if (commentTest.DatePosted != null)
            Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(commentTest.DatePosted)).Days < 1);
    }

    /// <summary>
    ///A test for GetImageExif
    ///</summary>
    [TestMethod()]
    public async Task GetImageExifPropertiesTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        string imageKey = imageTest.ImageKey; 
        string albumPassword = string.Empty; 
        string sitePassword = string.Empty; 
        ImageExif actual = await target.GetImageExif(imageId, imageKey, albumPassword, sitePassword);

        Assert.AreEqual("28/5", actual.Aperture, "Aperture");
        Assert.AreEqual(null, actual.Brightness, "Brightness");
        Assert.AreEqual(null, actual.CCDWidth, "CCDWidth");
        Assert.AreEqual(ColorSpace.sRGB, actual.ColorSpace, "ColorSpace");
        Assert.AreEqual(null, actual.CompressedBitsPerPixel, "CompressedBitsPerPixel");
        Assert.AreEqual(Contrast.Normal, actual.Contrast, "Contrast");
        Assert.AreEqual("2012-01-15 18:03:38", actual.DateTimeDigitized, "DateTimeDigitized");
        Assert.AreEqual("2012-02-14 23:59:58", actual.DateTimeModified, "DateTimeModified");
        Assert.AreEqual("2012-01-15 18:03:38", actual.DateTimeOriginal, "DateTimeOriginal");
        Assert.AreEqual(null, actual.DigitalZoomRatio, "DigitalZoomRatio");
        Assert.AreEqual("0", actual.ExposureBiasValue, "ExposureBiasValue");
        Assert.AreEqual(ExposureMode.AutoExposure, actual.ExposureMode, "ExposureMode");
        Assert.AreEqual(ExposureProgram.NormalProgram, actual.ExposureProgram, "ExposureProgram");
        Assert.AreEqual("1/60", actual.ExposureTime, "ExposureTime");
        Assert.AreEqual(Flash.FlashFiredCompulsoryFlashMode, actual.Flash, "Flash");
        Assert.AreEqual("47/1", actual.FocalLength, "FocalLength");
        Assert.AreEqual("761/10", actual.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
        Assert.AreEqual(400, actual.ISO, "ISO");
        Assert.AreEqual(LightSource.Unknown, actual.LightSource, "LightSource");
        Assert.AreEqual("Canon", actual.Make, "Make");
        Assert.AreEqual(Metering.Pattern, actual.Metering, "Metering");
        Assert.AreEqual("Canon EOS DIGITAL REBEL XS", actual.Model, "Model");
        Assert.AreEqual(Saturation.Normal, actual.Saturation, "Saturation");
        Assert.AreEqual(SensingMethod.Unknown, actual.SensingMethod, "SensingMethod");
        Assert.AreEqual(Sharpness.Normal, actual.Sharpness, "Sharpness");
        Assert.AreEqual(null, actual.SubjectDistance, "SubjectDistance");
        Assert.AreEqual(SubjectDistanceRange.Unknown, actual.SubjectDistanceRange, "SubjectDistanceRange");
        Assert.AreEqual(WhiteBalance.AutoWhiteBalance, actual.WhiteBalance, "WhiteBalance");
    }

    /// <summary>
    ///A test for GetImageExif
    ///</summary>
    [TestMethod()]
    public async Task GetImageExifTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId;
        string imageKey = imageTest.ImageKey;
        string albumPassword = string.Empty;
        string sitePassword = string.Empty;
        ImageExif actual = await target.GetImageExif(imageId, imageKey, albumPassword, sitePassword);
        Assert.IsNotNull(actual);
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
        Assert.AreEqual(0, actual.Altitude);
        Assert.AreEqual("This is a test Video", actual.Caption);
        if (actual.DateUploaded != null)
        {
            var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
            Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
        }
        else
            Assert.Fail("Image Uploaded Date was not provided, or is null");
        Assert.AreEqual("TestVideo.mov", actual.Filename);
        Assert.AreEqual("MP4", actual.Format);
        Assert.AreEqual(1080, actual.Height);
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
        Assert.AreEqual(0.0, actual.Latitude);
        Assert.AreEqual(0.0, actual.Longitude);
        Assert.IsNotNull(actual.MD5Sum);
        Assert.AreEqual(1, actual.PositionInAlbum);
        Assert.IsNotNull(actual.Revision);
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
        Assert.AreEqual(0, actual.Altitude, "Altitude");
        Assert.AreEqual("Pepper", actual.Caption, "Caption");
        if (actual.DateUploaded != null)
        {
            var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
            Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
        }
        else
            Assert.Fail("Image Uploaded Date was not provided, or is null");
        Assert.AreEqual("TestImage.jpg", actual.Filename, "Filename");
        Assert.AreEqual("JPG", actual.Format, "Format");
        Assert.AreEqual(427, actual.Height, "Height");
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
        Assert.AreEqual("47.61", actual.Latitude.ToString("F"), "Latitude");
        Assert.AreEqual("-122.33", actual.Longitude.ToString("F"), "Longitude");
        Assert.AreEqual("be8859c3de0d53c44d8d3d926e4637aa", actual.MD5Sum, "MD5Sum");
        Assert.AreEqual(1, actual.PositionInAlbum, "PositionInAlbum");
        Assert.IsNotNull(actual.Revision, "Revision");
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
    ///A test for MoveToAlbum
    ///</summary>
    [TestMethod()]
    public async Task MoveToAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        string imageKey = imageTest.ImageKey; 

        // Create the target album
        var albumMoveTarget = await Utility.CreateArbitraryTestAlbum(core, "TestAlbumMoveTarget");
        
        bool expected = true; 
        bool actual = await target.MoveToAlbum(imageId, imageKey, albumMoveTarget.AlbumId);
        Assert.AreEqual(expected, actual);

        // See if there are images in the target album
        var albumTarget = new AlbumService(core);
        var albumInfo = await albumTarget.GetAlbumDetail(albumMoveTarget.AlbumId, albumMoveTarget.AlbumKey);
        Assert.AreEqual(albumInfo.ImageCount, 1);

        // Clean up temporary Album
        var moveAlbumCheck = await Utility.RemoveArbitraryTestAlbum(core, "TestAlbumMoveTarget");
    }

    /// <summary>
    ///A test for Rotate
    ///</summary>
    [TestMethod()]
    public async Task RotateTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        var degrees = Degrees.NinetyDegrees; 
        bool flip = true; 
        bool expected = true; 
        bool actual;
        System.Threading.Thread.Sleep(10000);
        var imageOrig = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
        actual = await target.Rotate(imageId, degrees, flip);
        Assert.AreEqual(expected, actual);

        // Wait for process to complete
        int count = 0;
        ImageDetail? imageAfter = null;
        do
        {
            var img = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
            if (img.MD5Sum != imageOrig.MD5Sum)
            {
                imageAfter = img;
                count = 5;
            }
            else
            {
                count++;
                System.Threading.Thread.Sleep(1000);
            }

        } while (count < 5);
        Assert.IsNotNull(imageAfter);
        Assert.AreNotEqual(imageOrig.MD5Sum, imageAfter.MD5Sum);
        
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
        origImage.Altitude = 100;
        origImage.Caption = "New Caption";
        origImage.Filename = "NewFile.jpg";
        origImage.Hidden = true;
        origImage.Keywords = "A Keyword; AnotherKeyword";
        origImage.Latitude = 1;
        origImage.Longitude = 2;

        bool expected = true;
        bool actual = await target.UpdateImage(origImage);
        Assert.AreEqual(expected, actual);

        var updatedImage = await target.GetImageInfo(imageTest.ImageId, imageTest.ImageKey);
        Assert.AreEqual(origImage.Altitude, updatedImage.Altitude, "Altitude");
        Assert.AreEqual(origImage.Caption, updatedImage.Caption, "Caption");
        Assert.AreEqual(origImage.Filename, updatedImage.Filename, "Filename");
        Assert.AreEqual(origImage.Hidden, updatedImage.Hidden, "Hidden");
        Assert.AreEqual(origImage.Keywords, updatedImage.Keywords, "Keywords");
        Assert.AreEqual(origImage.Latitude, updatedImage.Latitude, "Latitude");
        Assert.AreEqual(origImage.Longitude, updatedImage.Longitude, "Longitude");
    }

    /// <summary>
    ///A test for ZoomThumbnail
    ///</summary>
    [TestMethod()]
    public async Task ZoomThumbnailTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var imageTest = await AddTestImage(core);
        var target = new ImageService(core);
        long imageId = imageTest.ImageId; 
        int height = 15; 
        int width = 0; 
        int x = 10; 
        int y = 10; 
        bool expected = true; 
        bool actual;
        actual = await target.ZoomThumbnail(imageId, height, width, x, y);
        Assert.AreEqual(expected, actual);
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
