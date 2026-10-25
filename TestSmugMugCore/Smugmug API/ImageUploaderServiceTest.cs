using System.Windows.Forms.VisualStyles;
using NuGet.Frameworks;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;
using SmugMugCore.Net.Service20;

namespace TestSmugMugCore.SmugMugAPI;

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
    /// Gets or sets the test context which provides
    /// information about and folder to find the test images/videos
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
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;
        var albumToCreate = new AlbumDetail()
        {
            Name = "ImageUploaderAlbumTest" + Random.Shared.Next(1000).ToString()
        };
        var createAlbumTask = albumService.CreateAlbum(albumToCreate);
        createAlbumTask.Wait();
        _albumTest = createAlbumTask.Result;

        // Give API time to persist images.
        Thread.Sleep(1000);
    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
        if (_albumTest != null)
        {
            var core = Utility.RetrieveSmugMugCore20();
            var albumService = core.AlbumService;

            var cleanupAlbumTask = albumService.DeleteAlbum(_albumTest);
            cleanupAlbumTask.Wait();
            _ = cleanupAlbumTask.Result;

            // Give API time to clean up.
            Thread.Sleep(1000);
        }
    }

    /// <summary>
    /// Upload a new image using the Image Uploader Service
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task UploadNewImageTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService;
        var contentService = core.ContentMetadataService;

        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await contentService.DiscoverMetadata(filename);

        string imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, content);
        Assert.IsNotNull(imageUri);
    }

    /// <summary>
    /// Upload a new image using the Image Uploader Service
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    [DeploymentItem(@"Content\TestScannedImage.jpg")]
    public async Task UploadUpdatedImageTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService;
        var contentService = core.ContentMetadataService;
        var albumImageService = core.AlbumImageService;

        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await contentService.DiscoverMetadata(filename);
        string imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, content);
        Assert.IsNotNull(imageUri);

        // Load the images for the album, and change the loaded image's title
        var albumImagesLoaded = await albumImageService.GetImageDetail(imageUri);

        string filenameUpdated = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestScannedImage.jpg");
        var contentUpdated = await contentService.DiscoverMetadata(filename);
        string imageUriUpdated = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, albumImagesLoaded.ImageKey, albumImagesLoaded.Serial, content);
        Assert.IsNotNull(imageUriUpdated);

        Thread.Sleep(1000); // Adding time for SmugMug Site to persist changes to cache
        Assert.AreEqual(imageUri, imageUriUpdated);
    }

    /// <summary>
    /// Upload the supported images that this uploader cares about, and verifies the name and metadata are uploaded.
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    [DeploymentItem(@"Content\TestJpegImage.jpeg")]
    [DeploymentItem(@"Content\TestPngImage.png")]
    [DeploymentItem(@"Content\TestScannedImage.jpg")]
    [DeploymentItem(@"Content\TestImageWithXPKeywords.jpg")]
    [DeploymentItem(@"Content\TestImageWithIPTCKeywords.jpg")]
    [DeploymentItem(@"Content\TestImageWithLightroomKeywords.jpg")]
    [DeploymentItem(@"Content\TestImageWithXPTitleCaption.jpg")]
    [DeploymentItem(@"Content\TestImageWithAsciiXPTitleCaption.jpg")]
    [DeploymentItem(@"Content\TestImageWithIPTCTitleCaption.jpg")]
    [DeploymentItem(@"Content\TestImageWithLightroomTitleCaption.jpg")]
    [DeploymentItem(@"Content\TestImageWithCaptionOnly.jpg")]
    public async Task UploadNewImageTest_Verify_Images()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService; 
        var albumImageService = core.AlbumImageService;
        var contentService = core.ContentMetadataService;
        AlbumImageDetail imageLoaded = null;
        FileMetaContent fileContent = null;
        string imageUri = null;

        // Check JPG
        string filenameJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImage.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pets; Pepper; Activity; Snow", imageLoaded.Keywords, "Keywords");

        // Check JPEG
        string filenameJPEG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestJpegImage.jpeg");
        fileContent = await contentService.DiscoverMetadata(filenameJPEG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestJpegImage.jpeg", imageLoaded.FileName);
        Assert.AreEqual("Pets; Pepper; Activity; Snow", imageLoaded.Keywords, "Keywords");

        // Check PNG
        string filenamePNG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestPngImage.png");
        fileContent = await contentService.DiscoverMetadata(filenamePNG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestPngImage.png", imageLoaded.FileName);
        Assert.AreEqual("", imageLoaded.Keywords, "Keywords"); // File format does not save metadata

        // Check Scanned JPG
        string filenameScannedJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestScannedImage.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameScannedJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestScannedImage.jpg", imageLoaded.FileName);
        Assert.AreEqual("Family; Idzi; Bob Idzi; Holidays; Christmas", imageLoaded.Keywords, "Keywords");
        Assert.AreEqual("Test Scanned Photo", imageLoaded.Title, "Title");
        // Windows Photo Gallery writes the title to the caption fields (dc:description, IPTC Caption), so no caption is sent.
        // SmugMug fills an empty caption from the file on upload, so verify what the sync reads instead.
        Assert.AreEqual("", fileContent.Caption ?? "", "Caption");

        // Check JPG with keywords only in the Windows XPKeywords
        string filenameWithXPKeywordsJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithXPKeywords.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithXPKeywordsJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithXPKeywords.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pets; Pepper; Activity; Snow", imageLoaded.Keywords, "Keywords");

        // Check JPG with keywords only in the IPTC Keywords (SmugMug would merge them as PetsPepper)
        string filenameWithIPTCKeywordsJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithIPTCKeywords.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithIPTCKeywordsJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithIPTCKeywords.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pets; Pepper; Activity; Snow", imageLoaded.Keywords, "Keywords");

        // Check JPG with Lightroom's keywords, names only in dc:subject/IPTC and the groupings in lr:hierarchicalSubject (ie: Pets|Pepper)
        string filenameWithLightroomKeywordsJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithLightroomKeywords.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithLightroomKeywordsJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithLightroomKeywords.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pepper; Snow; Pets; Activity", imageLoaded.Keywords, "Keywords");

        // Check JPG with the title/caption only in the Windows XPTitle/XPComment
        string filenameWithXPTitleCaptionJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithXPTitleCaption.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithXPTitleCaptionJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithXPTitleCaption.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pepper", imageLoaded.Title, "Title");
        Assert.AreEqual("Pepper in the snow", imageLoaded.Caption, "Caption");

        // Check JPG with the Windows XPTitle/XPComment written as ASCII instead of UCS-2 (would be garbled as 敐灰牥)
        string filenameWithAsciiXPTitleCaptionJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithAsciiXPTitleCaption.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithAsciiXPTitleCaptionJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithAsciiXPTitleCaption.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pepper", imageLoaded.Title, "Title");
        Assert.AreEqual("Pepper in the snow", imageLoaded.Caption, "Caption");

        // Check JPG with the title/caption only in the IPTC Object Name/Caption
        string filenameWithIPTCTitleCaptionJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithIPTCTitleCaption.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithIPTCTitleCaptionJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithIPTCTitleCaption.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pepper", imageLoaded.Title, "Title");
        Assert.AreEqual("Pepper in the snow", imageLoaded.Caption, "Caption");

        // Check JPG with Lightroom's title/caption, title in dc:title/IPTC and the caption also in the EXIF ImageDescription
        string filenameWithLightroomTitleCaptionJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithLightroomTitleCaption.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithLightroomTitleCaptionJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithLightroomTitleCaption.jpg", imageLoaded.FileName);
        Assert.AreEqual("Pepper", imageLoaded.Title, "Title");
        Assert.AreEqual("Pepper in the snow", imageLoaded.Caption, "Caption");

        // Check JPG with only a caption (ie: TagPup), the caption in the EXIF ImageDescription is not used as the title
        string filenameWithCaptionOnlyJPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImageWithCaptionOnly.jpg");
        fileContent = await contentService.DiscoverMetadata(filenameWithCaptionOnlyJPG);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
        Assert.AreEqual("TestImageWithCaptionOnly.jpg", imageLoaded.FileName);
        Assert.AreEqual("", imageLoaded.Title, "Title");
        Assert.AreEqual("Pepper in the snow", imageLoaded.Caption, "Caption");
    }


    /// <summary>
    /// Upload the supported videos that this uploader cares about, and verifies the name and metadata are uploaded.
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    [DeploymentItem(@"Content\TestVideoMp4.mp4")]
    [DeploymentItem(@"Content\TestVideoMpg.mpg")]
    [DeploymentItem(@"Content\TestVideoWmv.wmv")]
    public async Task UploadNewImageTest_Verify_Videos()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService;
        var albumImageService = core.AlbumImageService;
        var contentService = core.ContentMetadataService;
        AlbumImageDetail videoLoaded = null;
        FileMetaContent fileContent = null;
        string videoUri = null;

        // Check MOV
        string filenameMOV = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideo.mov");
        fileContent = await contentService.DiscoverMetadata(filenameMOV);
        videoUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        videoLoaded = await albumImageService.GetImageDetail(videoUri);
        Assert.AreEqual("TestVideo.mov", videoLoaded.FileName);
        Assert.AreEqual("Scenic; Animals; Activity; Cruise", videoLoaded.Keywords, "Keywords");

        // Check MP4
        string filenameMP4 = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoMp4.mp4");
        fileContent = await contentService.DiscoverMetadata(filenameMP4);
        videoUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        videoLoaded = await albumImageService.GetImageDetail(videoUri);
        Assert.AreEqual("TestVideoMp4.MP4", videoLoaded.FileName);
        Assert.AreEqual("", videoLoaded.Keywords, "Keywords"); // File format does not save metadata

        // Check MOV
        string filenameMPG = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoMpg.mpg");
        fileContent = await contentService.DiscoverMetadata(filenameMPG);
        videoUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        videoLoaded = await albumImageService.GetImageDetail(videoUri);
        Assert.AreEqual("TestVideoMpg.mpg", videoLoaded.FileName);
        Assert.AreEqual("", videoLoaded.Keywords, "Keywords"); // File format does not save metadata

        // Check WMV
        string filenameWMV = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestVideoWmv.wmv");
        fileContent = await contentService.DiscoverMetadata(filenameWMV);
        videoUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        videoLoaded = await albumImageService.GetImageDetail(videoUri);
        Assert.AreEqual("TestVideoWmv.wmv", videoLoaded.FileName);
        Assert.AreEqual("Activity; Snow; Pets; Pepper", videoLoaded.Keywords, "Keywords");
    }

    /// <summary>
    /// Verify that BMP files are invalid and fail to upload
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestBmpImage.bmp")]
    [ExpectedException(typeof(SmugMugException), "BMP file is not expected to be accepted")]
    public async Task UploadNewImageTest_Invalid_BMP()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService;
        var albumImageService = core.AlbumImageService;
        var contentService = core.ContentMetadataService;
        AlbumImageDetail imageLoaded = null;
        FileMetaContent fileContent = null;
        string imageUri = null;

        // Check BMP
        string filenameBMP = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestBmpImage.bmp");
        fileContent = await contentService.DiscoverMetadata(filenameBMP);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
    }

    /// <summary>
    /// Verify that TIF files are invalid and fail to upload
    /// </summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestTifImage.tif")]
    [ExpectedException(typeof(SmugMugException), "TIF file is not expected to be accepted")]
    public async Task UploadNewImageTest_Invalid_TIF()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var imageUploaderService = core.ImageUploaderService;
        var albumImageService = core.AlbumImageService;
        var contentService = core.ContentMetadataService;
        AlbumImageDetail imageLoaded = null;
        FileMetaContent fileContent = null;
        string imageUri = null;

        // Check TIF
        string filenameTIF = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestTifImage.tif");
        fileContent = await contentService.DiscoverMetadata(filenameTIF);
        imageUri = await imageUploaderService.UploadAlbumImage(_albumTest.AlbumKey, fileContent);
        imageLoaded = await albumImageService.GetImageDetail(imageUri);
    }
}
