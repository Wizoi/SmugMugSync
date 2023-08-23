namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for UserServiceTest and is intended
    ///to contain all UserServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class UserServiceTest
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
        ///A test for GetUserTree
        ///</summary>
        [TestMethod()]
        public void GetUserTreeNoAlbumDetailTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new UserService(core);
            var actual = target.GetUserTree(returnAlbumDetail:false);

            Assert.IsTrue(actual.Length > 0);

            // The first has an ID of 0, so we need to go ahead a bit
            var catTest = actual[5];
            Assert.IsTrue(catTest.CategoryId > 0);
            Assert.IsTrue(catTest.Name.Length > 0);
        }


        /// <summary>
        ///A test for GetUserTree
        ///</summary>
        [TestMethod()]
        public void GetUserTreeAlbumDetailTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new UserService(core);
            var actual = target.GetUserTree(returnAlbumDetail: true);

            Assert.IsTrue(actual.Length > 0);

            // The first has an ID of 0, so we need to go ahead a bit
            var catTest = actual[5];
            Assert.IsTrue(catTest.CategoryId > 0);
            Assert.IsTrue(catTest.Name.Length > 0);
        }

        /// <summary>
        ///A test for GetUserStats
        ///</summary>
        [TestMethod()]
        public void GetUserStatsTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            UserService target = new UserService(core); 
            int month = DateTime.Now.Month - 1; 
            int year = DateTime.Now.Year; 
            bool includeAlbums = true; 
            var actual = target.GetUserStats(month, year, includeAlbums);
            
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        [TestMethod()]
        public void GetUserTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            UserService target = new UserService(core); 
            string nickname = "idzifamily"; 
            var actual = target.GetUser(nickname);

            Assert.AreEqual(actual.NickName, nickname);
            Assert.IsTrue(actual.Name.Length > 0);
            Assert.IsTrue(actual.URL.Length > 0);
        }
    }
}
