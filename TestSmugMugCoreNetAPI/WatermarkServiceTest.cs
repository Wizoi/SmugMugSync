namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for WatermarkServiceTest and is intended
    ///to contain all WatermarkServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class WatermarkServiceTest
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
        ///A test for UpdateWatermark
        ///</summary>
        [TestMethod()]
        public void UpdateWatermarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            WatermarkService target = new WatermarkService(core); 
            Watermark watermark = null; 
            bool expected = false; 
            bool actual;
            actual = target.UpdateWatermark(watermark);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWatermarkList
        ///</summary>
        [TestMethod()]
        public void GetWatermarkListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            WatermarkService target = new WatermarkService(core); 
            bool includeDetails = false; 
            Watermark[] expected = null; 
            Watermark[] actual;
            actual = target.GetWatermarkList(includeDetails, Array.Empty<string>());
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWatermark
        ///</summary>
        [TestMethod()]
        public void GetWatermarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            WatermarkService target = new WatermarkService(core); 
            int watermarkId = 0; 
            Watermark expected = null; 
            Watermark actual;
            actual = target.GetWatermark(watermarkId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeleteWatermark
        ///</summary>
        [TestMethod()]
        public void DeleteWatermarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            WatermarkService target = new WatermarkService(core); 
            int watermarkId = 0; 
            bool expected = false; 
            bool actual;
            actual = target.DeleteWatermark(watermarkId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateWatermark
        ///</summary>
        [TestMethod()]
        public void CreateDeleteWatermarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            WatermarkService target = new WatermarkService(core); 
            Watermark expected = new Watermark();
            expected.Name = "Test Watermark";

            // Remove any leftover watermarks
            Utility.RemoveArbitraryTestWatermarks(core, expected.Name);

            var actual = target.CreateWatermark(expected);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.IsTrue(actual.WatermarkId > 0);

            // Cleanup afterwards
            Utility.RemoveArbitraryTestWatermarks(core, expected.Name);
        
        }
    }
}
