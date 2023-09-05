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
    ///A test for UpdateAlbum
    ///</summary>
    [TestMethod(), Obsolete("Updating the album no llonger  supported.")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task UpdateAlbumTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        var album = _albumTest;
        bool expected = true;
        album.Description = "This is a test";

        // Perform Update
        bool actual = await target.UpdateAlbum(album);
        Assert.AreEqual(expected, actual);

        // Reload album and verify change
        var reloadedAlbum = await target.GetAlbumDetail(album.AlbumId, album.AlbumKey);
        Assert.AreEqual(album.Description, reloadedAlbum.Description);
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
        var imageList = await imageService.GetAlbumImages(Array.Empty<string>(), _albumTest.AlbumId, _albumTest.AlbumKey);
        if (imageList.Images == null || imageList.ImageCount == 0)
            Assert.Fail("Images not found.");

        Assert.AreEqual(imageList.Images[0].ImageId, imageFirst.ImageId);
   }

    /// <summary>
    ///A test for Resort
    ///</summary>
    [TestMethod(), Obsolete("Updating the Image properties no longer supported")]
    [DeploymentItem(@"Content\TestImage.jpg")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task ResortTest()
    {
        if (this.TestContext == null)
            Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");    
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        var album = _albumTest;

        // Set the sort default values
        album.SortDirectionDescending = false;
        album.SortMethod = SortMethod.FileName;
        _ = await target.UpdateAlbum(album);

        // Put two images into the gallery
        var uploader = new ImageUploaderService(core);
        int albumId = _albumTest.AlbumId;
        string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
        var content = await core.ContentMetadataService.DiscoverMetadata(filename);
        content.Caption = "Second";
        var imageSecond = await uploader.UploadUpdatedImage(albumId, 0, content);

        content.Caption = "First";
        var imageFirst = await uploader.UploadUpdatedImage(albumId, 0, content);
        
        // Verify the current image placement
        var imageService = new ImageService(core);
        var imageList = await imageService.GetAlbumImages(Array.Empty<string>(), _albumTest.AlbumId, _albumTest.AlbumKey);
        if (imageList.Images == null || imageList.ImageCount == 0)
            Assert.Fail("Images not found.");

        Assert.AreEqual(imageList.Images[0].ImageId, imageSecond.ImageId);
        Assert.AreEqual(imageList.Images[1].ImageId, imageFirst.ImageId);

        // Change the sort
        album.SortDirectionDescending = true;
        album.SortMethod = SortMethod.FileName;
        _ = await target.UpdateAlbum(album);

        // Resort the album
        bool expected = true;
        bool actual = await target.Resort(album.AlbumId);
        Assert.AreEqual(expected, actual);

        // Verify the current image placement
        var imageAfterList = await imageService.GetAlbumImages(Array.Empty<string>(), _albumTest.AlbumId, _albumTest.AlbumKey);
        if (imageAfterList.Images == null || imageList.ImageCount == 0)
            Assert.Fail("Images not found.");

        Assert.AreEqual(imageAfterList.Images[1].ImageId, imageFirst.ImageId);
        Assert.AreEqual(imageAfterList.Images[0].ImageId, imageSecond.ImageId);
    }

    /// <summary>
    ///A test for GetCommentList
    ///</summary>
    [TestMethod(), Obsolete("Comment Lists no longer supported.")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task GetCommentListTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        int albumId = _albumTest.AlbumId; 
        string albumKey = _albumTest.AlbumKey; 

        _ = await target.AddComment(albumId, albumKey, Array.Empty<string>(), "Test Comment");

        var actual = await target.GetCommentList(albumId, albumKey);
        if (actual.Length == 0)
        {
            Assert.Fail("Test comment was not found.");
        }
    }

    /// <summary>
    ///A test for GetAlbumStats
    ///</summary>
    [TestMethod(), Obsolete("Album stats no longer supported.")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task GetAlbumStatsTest()
    {
        if (_albumTest == null)
            Assert.Fail("FATAL ERROR: Album  for Testing is not properly set by the test runner.");

        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        int albumId = _albumTest.AlbumId;
        int month = DateTime.Now.Month;
        int year = DateTime.Now.Year; 
        bool includImageInfo = true; 
        AlbumStats actual;
        actual = await target.GetAlbumStats(albumId, month, year, includImageInfo);
        Assert.IsNotNull(actual);
        Assert.AreEqual(actual.AlbumId, albumId);
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
        var actual = await target.GetAlbumList(Array.Empty<string>(), returnEmpty, nickName, sitePassword);
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
        var actual = await target.GetAlbumList(new string[] { "Keywords", "ImageCount" }, returnEmpty, nickName, sitePassword);
        Assert.IsFalse(actual.Length == 0);
    }

    /// <summary>
    ///A test for GetAlbumInfoList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumInfoListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        bool returnEmpty = false; 
        string nickName = string.Empty; 
        string sitePassword = string.Empty; 
        var actual = await target.GetAlbumDetailList(returnEmpty, nickName, sitePassword);
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
    public async void CreateDeleteAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core);
        var album = new AlbumDetail
        {
            Title = "TestAlbumCreate",
            BackprintingForPrints = "",
            BoutiquePackagingForOrders = BoutiquePackaging.No,
            CanRank = true,
            CleanDisplay = true,
            ColorCorrection = ColorCorrection.No,
            CommentsAllowed = true,
            Description = "This is a test album to verify my API",
            ExifAllowed = true,
            ExternalLinkAllowed = true,
            FamilyEditAllowed = true,
            FilenameDisplayWhenNoCaptions = true,
            FriendEditAllowed = true,
            GeographyMappingEnabled = true,
            HeaderDefaultIsSmugMug = true,
            HideOwner = true,
            InterceptShippingEnabled = InterceptShipping.No,
            Keywords = "test;test2",
            NiceName = "NiceNameTest",
            PackageBrandedOrdersEnabled = true
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
    [DeploymentItem("SmugMug.Net.dll")]
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
    [DeploymentItem("SmugMug.Net.dll")]
    public void IsValidNiceNameTest_InvalidDashFirst()
    {
        string niceName = "-KevinTest"; 
        bool expected = false; 
        bool actual;
        actual = SmugMug.Net.Core.SmugMugCore.IsValidNiceName(niceName);
        Assert.AreEqual(expected, actual);
    }


    /// <summary>
    ///A test for RemoveWatermark
    ///</summary>
    [TestMethod(), Obsolete("Watermarks no longer supported.")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task RemoveWatermarkTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        int albumId = -1; 
        bool expected = false; 
        bool actual;
        actual = await target.RemoveWatermark(albumId);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for ApplyWatermark
    ///</summary>
    [TestMethod(), Obsolete("Appplying watermarks no longer supported.")]
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public async Task ApplyWatermarkTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new AlbumService(core); 
        int albumId = -1; 
        int watermarkId = 0; 
        bool expected = false; 
        bool actual;
        actual = await target.ApplyWatermark(albumId, watermarkId);
        Assert.AreEqual(expected, actual);
        Assert.Inconclusive("Verify the correctness of this test method.");
    }
}
