namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for FanServiceTest and is intended
///to contain all FanServiceTest Unit Tests
///</summary>
[TestClass()]
public class FanServiceTest
{
    /// <summary>
    ///A test for GetFanList
    ///</summary>
    [TestMethod()]
    public void GetFanListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        var target = new FanService(core);
        var actual = target.GetFanList();

        // Not sure what to validate more specifically, but if it doens't error for now, then I'm OK with that :)
    }
}
