namespace TestSmugMugCoreNetAPI
{
    
    /// <summary>
    ///This is a test class for ThemeServiceTest and is intended
    ///to contain all ThemeServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class ThemeServiceTest
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
        ///A test for GetThemes
        ///</summary>
        [TestMethod()]
        public void GetThemesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ThemeService(core);
            var actual = target.GetThemes();

            // There will be some smugmug default themes to test against
            Assert.IsTrue(actual.Length > 0);

            var theme = actual[0];
            Assert.IsTrue(theme.Name.Length > 0);
            Assert.IsTrue(theme.ThemeId > 0);
            Assert.IsTrue(theme.Type == SmugMug.Net.Data.Domain.Theme.ThemeType.SmugMug);
        }
    }
}
