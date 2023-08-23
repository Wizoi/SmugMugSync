namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for PrintmarkServiceTest and is intended
    ///to contain all PrintmarkServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class PrintmarkServiceTest
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
        ///A test for CreatePrintmark
        ///</summary>
        [TestMethod()]
        public void CreateDeletePrintmarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new PrintmarkService(core);
            var expected = new PrintmarkInfo();
            expected.Name = "Test Create Printmark";
            
            // Clean up old Test Printmarks
            Utility.RemoveArbitraryTestPrintmarks(core, expected.Name);

            // Create Printmark
            PrintmarkInfo actual;
            actual = target.CreatePrintmark(expected);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.IsTrue(actual.PrintmarkId > 0);

            // Clean up when done
            Utility.RemoveArbitraryTestPrintmarks(core, expected.Name);
        }


        /// <summary>
        ///A test for GetPrintmark
        ///</summary>
        [TestMethod()]
        public void GetPrintmarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            PrintmarkService target = new PrintmarkService(core); 
            string printmarkId = string.Empty; 
            PrintmarkInfo expected = null; 
            PrintmarkInfo actual;
            actual = target.GetPrintmark(printmarkId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPrintmarkInfoList
        ///</summary>
        [TestMethod()]
        public void GetPrintmarkInfoListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new PrintmarkService(core); 
            var actual = target.GetPrintmarkInfoList();

            // Cannot test this, I do not have a pro account :(
        }

        /// <summary>
        ///A test for GetPrintmarkList
        ///</summary>
        [TestMethod()]
        public void GetPrintmarkListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new PrintmarkService(core);
            var actual = target.GetPrintmarkList(Array.Empty<string>());

            // Cannot test this, I do not have a pro account :(
        }

        /// <summary>
        ///A test for UpdatePrintmark
        ///</summary>
        [TestMethod()]
        public void UpdatePrintmarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new PrintmarkService(core);
            PrintmarkInfo printmark = null; 
            bool expected = false; 
            bool actual;
            actual = target.UpdatePrintmark(printmark);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
