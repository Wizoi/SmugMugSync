using SmugMug.Net.Data.Domain.Album;
namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for AlbumServiceTest and is intended
///to contain all AlbumServiceTest Unit Tests
///</summary>
[TestClass()]
public class AlbumServiceTest
{
    private static AlbumDetail? _albumTest = null;
    private static int _i = 0;
    private int _iteration = 0;


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
    [TestInitialize()]
    public void MyTestInitialize()
    {
        _iteration = _i++;

        var core = Utility.RetrieveSmugMugCore();
        var createTestAlbumTask = Utility.CreateArbitraryTestAlbum(core, "TestAlbum" + _iteration.ToString());
        createTestAlbumTask.Wait();
        _albumTest = createTestAlbumTask.Result;

    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [TestCleanup()]
    public void MyTestCleanup()
    {
        var core = Utility.RetrieveSmugMugCore();
        Utility.RemoveArbitraryTestAlbum(core, "TestAlbum").Wait();
    }

    /// <summary>
    /// Upload image to Album
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task AlbumImageUploadTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();

        // Put two images into the gallery
        var uploader = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        content.Caption = "First";
        var imageFirst = await uploader.UploadUpdatedImage(albumId, 0, content);

        // Verify the image 
        var imageService = new ImageService(core);
        var imageList = await imageService.GetAlbumImages([], _albumTest.AlbumId, _albumTest.AlbumKey);
        if (imageList.Images == null || imageList.ImageCount == 0)
            Assert.Fail("Images not found.");

        Assert.AreEqual(imageList.Images[0].ImageId, imageFirst.ImageId);
   }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        bool returnEmpty = false; 
        string nickName = string.Empty; 
        string sitePassword = string.Empty; 
        var actual = await target.GetAlbumList([], returnEmpty, nickName, sitePassword);
        Assert.IsFalse(actual.Length == 0);
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListExtraTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        bool returnEmpty = false;
        string nickName = string.Empty;
        string sitePassword = string.Empty;
        var actual = await target.GetAlbumList(["Keywords", "ImageCount"], returnEmpty, nickName, sitePassword);
        Assert.IsFalse(actual.Length == 0);
    }

    /// <summary>
    ///A test for GetAlbumInfo
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumInfoTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        var expected = _albumTest;
        var actual = await target.GetAlbumDetail(expected.AlbumId, expected.AlbumKey);
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Title, actual.Title);
    }


    /// <summary>
    ///A test for CreateAlbum
    ///</summary>
    [TestMethod()]
    public async Task CreateDeleteAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        var album = new AlbumDetail
        {
            Title = "TestAlbumCreate",
            CanRank = true,
            CommentsAllowed = true,
            ExifAllowed = true,
            GeographyMappingEnabled = true,
            NiceName = "NiceNameTest",
        };
        AlbumDetail actual;

        // Remove the album if it already exists
        await Utility.RemoveArbitraryTestAlbum(core, "TestAlbumCreate");
        
        actual = await target.CreateAlbum(album);

        // Clean up the album afterwards
        _ = await target.DeleteAlbum(actual.AlbumId);
    }

    /// <summary>
    ///A test for IsValidNiceName
    ///</summary>
    [TestMethod()]
    public void IsValidNiceNameTest_Valid()
    {
        string niceName = "KevinTest"; 
        bool expected = true; 
        bool actual;
        actual = SmugMug.Net.Core.SmugMugCore.IsValidNiceName(niceName);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for IsValidNiceName
    ///</summary>
    [TestMethod()]
    public void IsValidNiceNameTest_InvalidDashFirst()
    {
        string niceName = "-KevinTest"; 
        bool expected = false; 
        bool actual;
        actual = SmugMug.Net.Core.SmugMugCore.IsValidNiceName(niceName);
        Assert.AreEqual(expected, actual);
    }
}
