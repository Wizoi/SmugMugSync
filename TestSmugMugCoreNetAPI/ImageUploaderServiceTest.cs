using System.Runtime.InteropServices;

namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for ImageUploaderServiceTest and is intended
///to contain all ImageUploaderServiceTest Unit Tests
///</summary>
[TestClass()]
public class ImageUploaderServiceTest
{
    private static AlbumDetail? _albumTest = null;
    private TestContext? testContextInstance;

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
        var core = Utility.RetrieveSmugMugCore();
        var createAlbumTask = Utility.CreateArbitraryTestAlbum(core, "TestAlbum");
        createAlbumTask.Wait();
        _albumTest = createAlbumTask.Result;
    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
        var core = Utility.RetrieveSmugMugCore();
        var cleanupAlbumTask = Utility.RemoveArbitraryTestAlbum(core, "TestAlbum");
        cleanupAlbumTask.Wait();
        _ = cleanupAlbumTask.Result;
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task UploadNewImageTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core); 
        int albumId = _albumTest.AlbumId; 
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);

        _ = await target.UploadNewImage(albumId, content);
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task UploadNewImagePropertiesTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);

        var imageContent = await target.UploadNewImage(albumId, content);

        // Verify the properties
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        var service = new ImageService(core);
        var info = await service.GetImageInfo(imageContent.ImageId, imageContent.ImageKey);
        Assert.AreEqual(content.Title, info.Caption, "Caption");
        Assert.AreEqual("Pets; Pepper; Activity; Snow", info.Keywords, "Keywords");
    }

    /// <summary>
    ///A test for UploadNewImage
    /// BUG: SmugMug Bug where Keywords are not uploaded
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    public async Task UploadNewVideoPropertiesTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideo.mov");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);

        var imageContent = await target.UploadNewImage(albumId, content);

        // Verify the properties
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        var service = new ImageService(core);
        var info = await service.GetImageInfo(imageContent.ImageId, imageContent.ImageKey);
        Assert.AreEqual(content.Title, info.Caption);
        Assert.AreEqual("Activity", info.Keywords, "Keywords");
    }

    /// <summary>
    ///A test for UploadNewVideo
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    public async Task UploadNewVideoTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideo.mov");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        _ =  await target.UploadNewImage(albumId, content);
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task UploadUpdatedImageTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");

        // Upload Initial Image
        var imageMetadata = await core.ContentMetadataService.DiscoverMetadata(filename);
        var imageUploaded = await target.UploadNewImage(albumId, imageMetadata);

        // Upload Updated Image
        var imageUpdated = await target.UploadUpdatedImage(albumId, imageUploaded.ImageId, imageMetadata);

        Assert.IsNotNull(imageUploaded, "Image Uploaded");
        Assert.IsNotNull(imageUpdated, "Image Updated");
        Assert.AreEqual(imageUploaded.URL, imageUpdated.URL, "ImageUpdated URL should be same URL");
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestJpegImage.jpeg")]
    [DeploymentItem(@"Content\TestPngImage.png")]
    [DeploymentItem(@"Content\TestScannedImage.jpg")]
    public async Task UploadUpdatedImageAllTypesTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        
        string filename;
        ImageUpload imageUploaded;

        // Upload Jpeg Image
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestJpegImage.jpeg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "JpegImage - ImageKey is empty, Image upload failed.");

        // Upload Png Image
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestPngImage.png");
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "PngImage - ImageKey is empty, Image upload failed.");

        // Upload Scanned Image
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestScannedImage.jpg");
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "ScannedImage - ImageKey is empty, Image upload failed.");
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestBmpImage.bmp")]
    [ExpectedException(typeof(SmugMugException), "BMP file is not expected to be accepted")]
    public async Task UploadUpdatedImageInvalidBmpTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;

        string filename;
        ImageUpload imageUploaded;

        // Upload Bmp Image
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestBmpImage.bmp");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "BmpImage - ImageKey is empty, Image upload failed.");
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestTifImage.tif")]
    [ExpectedException(typeof(SmugMugException), "TIF fie is not expected to be accepted")]
    public async Task UploadUpdatedImageInvalidTifTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;

        string filename;
        ImageUpload imageUploaded;

        // Upload Tif Image
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestTifImage.tif");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "TifImage - ImageKey is empty, Image upload failed.");
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    [ExpectedException(typeof(SmugMugException), "Cannot upload image/video over another, exception expected.")]
    public async Task UploadUpdatedVideoTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideo.mov");

        // Upload Initial Image
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        var imageUploaded = await target.UploadNewImage(albumId, content);

        // Upload Updated Image - EXCEPTION.
        var imageUpdated = await target.UploadUpdatedImage(albumId, imageUploaded.ImageId, content);

        Assert.IsNotNull(imageUploaded);
        Assert.IsNotNull(imageUpdated);
        Assert.AreEqual(imageUploaded.URL, imageUpdated.URL);
    }

    /// <summary>
    ///A test for UploadNewImage
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoMp4.mp4")]
    [DeploymentItem(@"Content\TestVideoMpg.mpg")]
    [DeploymentItem(@"Content\TestVideoWmv.wmv")]
    public async Task UploadUpdatedVideoAllTypesTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename;
        ImageUpload imageUploaded;

        // Upload MP4 Video
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoMp4.mp4");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "Mp4Video - ImageKey is empty, Video upload failed.");

        // Upload MPG Video
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoMpg.mpg");
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "MpgVideo - ImageKey is empty, Video upload failed.");

        // Upload Wmv Video
        filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoWmv.wmv");
        content = await core.ContentMetadataService.DiscoverMetadata(filename);
        imageUploaded = await target.UploadNewImage(albumId, content);
        Assert.IsNotNull(imageUploaded.ImageKey, "WmvVideo - ImageKey is empty, Video upload failed.");
    }
}
