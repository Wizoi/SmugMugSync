using System.Runtime.InteropServices;

namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for FamilyServiceTest and is intended
///to contain all FamilyServiceTest Unit Tests
///</summary>
[TestClass()]
public class FamilyServiceTest
{
    private string testFamily = "kilbournephoto";

    /// <summary>
    ///A test for AddFamily
    ///</summary>
    [TestMethod()]
    public async Task AddRemoveFamilyTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        FamilyService target = new FamilyService(core);
        string nickName = testFamily;
        bool actual;
        
        // Clean up old data
        await Utility.RemoveArbitraryTestFamilies(core, nickName);

        actual = await target.AddFamily(nickName);
        Assert.AreEqual(actual, true);

        // Clean up when done
        await Utility.RemoveArbitraryTestFamilies(core, nickName);

    }

    /// <summary>
    ///A test for GetFamilyList
    ///</summary>
    [TestMethod()]
    public async Task GetFamilyListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        FamilyService target = new FamilyService(core);
        var actual = await target.GetFamilyList();
        
        // Not sure easy way to repeatedly verify this, manually tested this once and verified Serialization and API (2/11/2012)
    }
}
