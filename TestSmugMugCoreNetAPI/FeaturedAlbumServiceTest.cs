namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for FeaturedAlbumServiceTest and is intended
    ///to contain all FeaturedAlbumServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class FeaturedAlbumServiceTest
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
        ///A test for GetFeaturedAlbumInfoList
        ///</summary>
        [TestMethod()]
        public void GetFeaturedAlbumInfoListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new FeaturedAlbumService(core); 
            string nickName = string.Empty; 
            string sitePassword = string.Empty; 
            var actual = target.GetFeaturedAlbumDetailList(nickName, sitePassword);

            if (actual.Length > 0)
            {
                var randomTest = actual[0];
                Assert.IsTrue(randomTest.AlbumId != 0);
                Assert.IsTrue(randomTest.AlbumKey != "");

                Assert.IsTrue(randomTest.Title.Length > 0);
                Assert.IsTrue(randomTest.Url.Length > 0);
            }
        }

        /// <summary>
        ///A test for GetFeaturedAlbumList
        ///</summary>
        [TestMethod()]
        public void GetFeaturedAlbumListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new FeaturedAlbumService(core);
            string nickName = string.Empty; 
            string sitePassword = string.Empty; 
            var actual = target.GetFeaturedAlbumList(Array.Empty<string>(), nickName, sitePassword);

            if (actual.Length > 0)
            {
                var randomTest = actual[0];
                Assert.IsTrue(randomTest.AlbumId != 0);
                Assert.IsTrue(randomTest.AlbumKey != "");
            }
        }
    }
}
