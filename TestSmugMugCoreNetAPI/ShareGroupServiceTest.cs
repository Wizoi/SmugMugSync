namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for ShareGroupServiceTest and is intended
    ///to contain all ShareGroupServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class ShareGroupServiceTest
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
        ///A test for Browse
        ///</summary>
        [TestMethod()]
        public void BrowseTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            string shareGroupTag = string.Empty; 
            string password = string.Empty; 
            Uri expected = null; 
            Uri actual;
            actual = target.Browse(shareGroupTag, password);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateShareGroupTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            ShareGroup shareGroup = null; 
            ShareGroup expected = null; 
            ShareGroup actual;
            actual = target.CreateSharegroup(shareGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetShareGroupAlbums
        ///</summary>
        [TestMethod()]
        public void GetShareGroupAlbumsTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            string shareGroupTag = string.Empty; 
            string password = string.Empty; 
            //var expected = null; 
            var actual = target.GetShareGroupAlbums(Array.Empty<string>(), shareGroupTag, password);
            Assert.AreEqual(null, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetShareGroupList
        ///</summary>
        [TestMethod()]
        public void GetShareGroupListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            bool includeAlbums = false; 
            ShareGroup[] expected = null; 
            ShareGroup[] actual;
            actual = target.GetShareGroupList(includeAlbums);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RemoveAlbumFromShareGroup
        ///</summary>
        [TestMethod()]
        public void RemoveAlbumFromShareGroupTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            string shareGroupId = string.Empty; 
            string albumId = string.Empty; 
            bool expected = false; 
            bool actual;
            actual = target.RemoveAlbumFromShareGroup(shareGroupId, albumId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UpdateShareGroup
        ///</summary>
        [TestMethod()]
        public void UpdateShareGroupTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            ShareGroupService target = new ShareGroupService(core); 
            ShareGroup shareGroup = null; 
            bool expected = false; 
            bool actual;
            actual = target.UpdateShareGroup(shareGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
