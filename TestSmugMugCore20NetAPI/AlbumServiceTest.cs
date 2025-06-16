using NuGet.Frameworks;
using SmugMug.Net.Core20;
using SmugMug.Net.Data20;
using SmugMug.Net.Service20;

namespace TestSmugMugCore20NetAPI;

/// <summary>
///This is a test class for AlbumServiceTest and is intended
///to contain all AlbumServiceTest Unit Tests
///</summary>
[TestClass()]
public class AlbumServiceTest
{
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
        var core = Utility.RetrieveSmugMugCore20();

    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [TestCleanup()]
    public void MyTestCleanup()
    {
        var core = Utility.RetrieveSmugMugCore20();
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListTestFull()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        bool returnEmpty = false;
        string nickName = string.Empty;
        string sitePassword = string.Empty;
        var actual = await target.GetAlbumListFull();
        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListTest_FilteredName()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        var actual = await target.GetAlbumListNamesOnly("");

        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].Name));
        Assert.IsTrue(string.IsNullOrEmpty(actual[0].Description));
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListTest_FilteredNameYear2024()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        var actual = await target.GetAlbumListNamesOnly("2024");

        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].Name));
        Assert.IsTrue(string.IsNullOrEmpty(actual[0].Description));
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumTest_LoadById()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        // Search for some albums
        var preload = await target.GetAlbumListNamesOnly("2024");
        Assert.IsFalse(preload.Length == 0);
        string albumKey = preload[0].AlbumKey;

        var actual = await target.GetAlbumDetail(albumKey);
        Assert.IsTrue(!string.IsNullOrEmpty(actual.AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual.Name));
    }

    [TestMethod()]
    public async Task CreateAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        // Search for some albums
        var preload = await target.GetAlbumListNamesOnly("2024");
        Assert.IsFalse(preload.Length == 0);
        string albumKey = preload[0].AlbumKey;

        var album = new AlbumDetail()
        {
            SortDirection = "Descending",
            SortMethod = "Date Taken",
            LargestSize = "Original",
            EXIF = true,
            Comments = true,
            CanRank = true,
            CanShare = true,
            Privacy = "Public",
            Geography = true,
            Name = "CreateAlbumTest: New Test Album!@#"
        };
        var actual = await target.CreateAlbum(album);
        Assert.AreEqual(album.Name, actual.Name, "Created album title does not match the given title");
        Assert.AreEqual("CreateAlbumTest-New-Test-Album", actual.NiceName, "Verify the nicename is clean and url safe");

        // Clean up Album 
        await target.DeleteAlbum(actual);

    }

    [TestMethod()]
    public async Task DeleteAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var target = core.AlbumService;

        // Create Album to Delete
        var albumToCreate = new AlbumDetail()
        {
            Name = "DeleteAlbumTest" + Random.Shared.Next(100).ToString()
        };
        var albumToDelete = await target.CreateAlbum(albumToCreate);

        // Run actual test
        var actual = await target.DeleteAlbum(albumToDelete);
        Assert.IsTrue(actual.Contains(albumToDelete.AlbumKey));
    }
}
