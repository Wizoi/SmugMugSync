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
    public void AddRemoveFamilyTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        FamilyService target = new FamilyService(core);
        string nickName = testFamily;
        bool actual;
        
        // Clean up old data
        Utility.RemoveArbitraryTestFamilies(core, nickName);

        actual = target.AddFamily(nickName);
        Assert.AreEqual(actual, true);

        // Clean up when done
        Utility.RemoveArbitraryTestFamilies(core, nickName);

    }

    /// <summary>
    ///A test for GetFamilyList
    ///</summary>
    [TestMethod()]
    public void GetFamilyListTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        FamilyService target = new FamilyService(core);
        var actual = target.GetFamilyList();
        
        // Not sure easy way to repeatedly verify this, manually tested this once and verified Serialization and API (2/11/2012)
    }

    /// <summary>
    /// A test for RemoveAll
    /// Manually tested on 2/11/2012 - Not sure I want this in normal test suite due to destructiveness for my account.
    ///</summary>
    [TestMethod(), Ignore()]
    public void RemoveAllTest()
    {
        var core = Utility.RetrieveSmugMugCore();
        FamilyService target = new FamilyService(core);
        bool expected = true; 
        bool actual = false;
        
        // Commented out to not cause harm, manually validated this works.
        //actual = target.RemoveAll();

        Assert.AreEqual(expected, actual);
    }
}
