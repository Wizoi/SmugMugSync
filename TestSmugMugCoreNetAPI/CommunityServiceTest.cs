namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for CommunityServiceTest and is intended
    ///to contain all CommunityServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommunityServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetCommunityList
        ///</summary>
        [TestMethod()]
        public void GetCommunityListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CommunityService target = new CommunityService(core); 
            var actual = target.GetCommunityList();
            
            // Not sure what to validate more specifically, but if it doens't error for now, then I'm OK with that :)
        }
    }
}
