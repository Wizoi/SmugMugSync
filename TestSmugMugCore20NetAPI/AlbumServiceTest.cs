namespace TestSmugMugCore20NetAPI;

/// <summary>
///This is a test class for AlbumServiceTest and is intended
///to contain all AlbumServiceTest Unit Tests
///</summary>
[TestClass()]
public class AlbumServiceTest
{
    /// <summary>
    /// Any setup code for the tests
    /// </summary>
    [TestInitialize()]
    public void MyTestInitialize()
    {
    }

    /// <summary>
    /// Any cleanup code at the end of the test
    /// </summary>
    [TestCleanup()]
    public void MyTestCleanup()
    {
    }

    /// <summary>
    /// Get the Albums with full properties
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumListTestFull()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        var actual = await albumService.GetAlbumListFull();
        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
    }

    /// <summary>
    /// Get the Album filtering on only certain key properties
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumListTest_FilteredName()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        var actual = await albumService.GetAlbumListNamesOnly("");

        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].Name));
        Assert.IsTrue(string.IsNullOrEmpty(actual[0].Description));
    }

    /// <summary>
    /// Get the album but search for a string in the name; also verify paging.
    /// Note: Need to have existing galleries for this, with the search term for valid test
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumListTest_FilteredNameYear2024()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        var actual = await albumService.GetAlbumListNamesOnly("2024");

        Assert.IsFalse(actual.Length == 0);
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual[0].Name));
        Assert.IsTrue(string.IsNullOrEmpty(actual[0].Description));
    }

    /// <summary>
    /// Test loading an album using the AlbumKey
    /// </summary>
    [TestMethod()]
    public async Task GetAlbumTest_LoadById()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        // Search for some albums
        var preload = await albumService.GetAlbumListNamesOnly("2024");
        Assert.IsFalse(preload.Length == 0);
        string albumKey = preload[0].AlbumKey;

        var actual = await albumService.GetAlbumDetail(albumKey);
        Assert.IsTrue(!string.IsNullOrEmpty(actual.AlbumKey));
        Assert.IsTrue(!string.IsNullOrEmpty(actual.Name));
    }

    /// <summary>
    /// Create a new Album
    /// </summary>
    [TestMethod()]
    public async Task CreateAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        // Search for some albums
        var preload = await albumService.GetAlbumListNamesOnly("2024");
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
        var actual = await albumService.CreateAlbum(album);
        Assert.AreEqual(album.Name, actual.Name, "Created album title does not match the given title");
        Assert.AreEqual("CreateAlbumTest-New-Test-Album", actual.NiceName, "Verify the nicename is clean and url safe");

        // Clean up Album 
        await albumService.DeleteAlbum(actual);

    }

    /// <summary>
    /// Delete an Album
    /// </summary>
    [TestMethod()]
    public async Task DeleteAlbumTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        var albumService = core.AlbumService;

        // Create Album to Delete
        var albumToCreate = new AlbumDetail()
        {
            Name = "DeleteAlbumTest" + Random.Shared.Next(100).ToString()
        };
        var albumToDelete = await albumService.CreateAlbum(albumToCreate);

        // Run actual test
        var actual = await albumService.DeleteAlbum(albumToDelete);
        Assert.IsTrue(actual.Contains(albumToDelete.AlbumKey));
    }
}
