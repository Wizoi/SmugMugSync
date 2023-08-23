namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for AccountServiceTest and is intended
    ///to contain all AccountServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class AccountServiceTest
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
        /// A test for Browse
        /// Checking for no exception or failure
        ///</summary>
        [TestMethod()]
        public void BrowseTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            AccountService target = new AccountService(core);
            string nickName = "idzifamily";
            string sitePassword = "lolo"; 
            target.Browse(nickName, sitePassword);
        }

    }
}
