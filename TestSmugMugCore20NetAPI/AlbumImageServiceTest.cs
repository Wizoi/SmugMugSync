using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NuGet.Frameworks;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;
using SmugMugCore.Net.Service20;

namespace TestSmugMugCore20NetAPI;

/// <summary>
/// This is a test class for validating the AlbumImage Service
///</summary>
[TestClass()]
public class AlbumImageServiceTest
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
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
    }

    [TestInitialize]
    public void RunBeforeEachTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;
        var uploaderService = core.ImageUploaderService;
        var contentService = core.ContentMetadataService;
        var albumToCreate = new AlbumDetail()
        {
            Name = "ImageUploaderAlbumTest" + Random.Shared.Next(100).ToString()
        };
        var createAlbumTask = albumService.CreateAlbum(albumToCreate);
        createAlbumTask.Wait();
        _albumTest = createAlbumTask.Result;

        string filename = System.IO.Path.Combine(TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = contentService.DiscoverMetadata(filename);
        content.Wait();
        (uploaderService.UploadAlbumImage(_albumTest.AlbumKey, content.Result)).Wait();
        (uploaderService.UploadAlbumImage(_albumTest.AlbumKey, content.Result)).Wait();
        (uploaderService.UploadAlbumImage(_albumTest.AlbumKey, content.Result)).Wait();
    }

    [TestCleanup]
    public void RunAfterEachTest()
    {
        if (_albumTest != null)
        {
            var core = Utility.RetrieveSmugMugCore20();
            var albumService = core.AlbumService;

            var cleanupAlbumTask = albumService.DeleteAlbum(_albumTest);
            cleanupAlbumTask.Wait();
            _ = cleanupAlbumTask.Result;
        }
    }

    /// <summary>
    /// A test to delete the image using the AlbumImage Service
    /// </summary>
    [TestMethod()]
    public async Task DeleteImage()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var albumImageService = core.AlbumImageService;

        // Add the image and retrieve its object
        var imagesLoaded = await albumImageService.GetAlbumImageListShort(_albumTest.AlbumKey);

        // Delete the image
        var imageDeleted = await albumImageService.DeleteImage(imagesLoaded[0].Uris.Image.Uri);
        Assert.IsNotNull(imageDeleted);
    }

    /// <summary>
    /// A test to Get all of the album image details from the AlbumImage Service
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumImagesFull()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var albumImageService = core.AlbumImageService;

        var albumImagesLoaded = await albumImageService.GetAlbumImageListFull(_albumTest.AlbumKey);
        Assert.AreEqual(3, albumImagesLoaded.Length);
    }

    /// <summary>
    /// A test to get a subset of album image properties from the from the AlbumImage Service
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumImagesShort()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var albumImageService = core.AlbumImageService;

        var albumImagesLoaded = await albumImageService.GetAlbumImageListShort(_albumTest.AlbumKey);
        Assert.AreEqual(3, albumImagesLoaded.Length);
    }

    /// <summary>
    /// Update properties for an album image (Title, Caption or Keywords)
    /// </summary>
    [TestMethod()]
    public async Task UpdateAlbumImages()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var albumImageService = core.AlbumImageService;

        var albumImagesLoaded = await albumImageService.GetAlbumImageListShort(_albumTest.AlbumKey);
        Assert.AreEqual(3, albumImagesLoaded.Length);

        var testAlbumImage = albumImagesLoaded[0];
        testAlbumImage.Title = "Updating the Comment";
        testAlbumImage.Caption = "Updating the Caption";
        testAlbumImage.Keywords = "New; Keywords";
        var updatedAlbumImage = await albumImageService.UpdateAlbumImage(testAlbumImage);
        Assert.AreEqual(testAlbumImage.Title, updatedAlbumImage.Title);
        Assert.AreEqual(testAlbumImage.Caption, updatedAlbumImage.Caption);
        Assert.AreEqual(testAlbumImage.Keywords, updatedAlbumImage.Keywords);
    }

    /// <summary>
    /// Download the AlbumImage to a local file
    /// </summary>
    [TestMethod()]
    public async Task DownloadAlbumImage()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album to test is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore20();
        var albumImageService = core.AlbumImageService;

        // Retrieve the recently loaded image metadata
        var albumImagesLoaded = await albumImageService.GetAlbumImageListShort(_albumTest.AlbumKey);
        Assert.IsTrue(albumImagesLoaded.Length == 3);
        var testAlbumImage = albumImagesLoaded[0];

        string localPath = Path.GetTempFileName();
        var isSuccess = await albumImageService.DownloadPrimaryImage(testAlbumImage, localPath);

        var fi = new FileInfo(localPath);
        long actual = fi.Length;

        // Cleanup file
        fi.Delete();

        Assert.AreEqual(testAlbumImage.ArchivedSize, actual);
    }
}
