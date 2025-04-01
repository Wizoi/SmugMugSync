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
//    private static AlbumDetail? _albumTest = null;
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
        //_iteration = _i++;

        var core = Utility.RetrieveSmugMugCore20();
       // var createTestAlbumTask = Utility.CreateArbitraryTestAlbum(core, "TestAlbum" + _iteration.ToString());
        //createTestAlbumTask.Wait();
        //_albumTest = createTestAlbumTask.Result;

    }

    /// <summary>
    /// When class is done, remove the test album created at the beginning
    /// </summary>
    [TestCleanup()]
    public void MyTestCleanup()
    {
        var core = Utility.RetrieveSmugMugCore20();
        //Utility.RemoveArbitraryTestAlbum(core, "TestAlbum").Wait();
    }

    /// <summary>
    ///A test for GetAlbumList
    ///</summary>
    [TestMethod()]
    public async Task GetAlbumListTest()
    {
        var core = Utility.RetrieveSmugMugCore20();

        var target = new AlbumService(core);
        bool returnEmpty = false; 
        string nickName = string.Empty; 
        string sitePassword = string.Empty; 
        //var actual = await target.GetAlbumList([], returnEmpty, nickName, sitePassword);
        //Assert.IsFalse(actual.Length == 0);
    }

    // 
}
