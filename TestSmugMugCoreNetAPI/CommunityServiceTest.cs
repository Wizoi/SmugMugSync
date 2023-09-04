namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for CommunityServiceTest and is intended
///to contain all CommunityServiceTest Unit Tests
///</summary>
[TestClass()]
public class CommunityServiceTest
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
    ///A test for GetCommunityList
    ///</summary>
    [TestMethod()]
    public async Task GetCommunityListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        CommunityService target = new CommunityService(core); 
        var actual = await target.GetCommunityList();
        
        // Not sure what to validate more specifically, but if it doens't error for now, then I'm OK with that :)
    }
}
