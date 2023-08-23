namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for FanServiceTest and is intended
    ///to contain all FanServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FanServiceTest
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
}
