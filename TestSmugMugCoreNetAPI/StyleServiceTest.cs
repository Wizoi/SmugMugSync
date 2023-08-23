namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for StyleServiceTest and is intended
    ///to contain all StyleServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class StyleServiceTest
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
        ///A test for GetTemplates
        ///</summary>
        [TestMethod()]
        public void GetTemplatesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new StyleService(core);
            var actual = target.GetTemplates();

            // There will be some smugmug default themes to test against
            Assert.IsTrue(actual.Length > 0);

            // There are system Templates, the first one has an index of 0, so we'll jump ahead a bit
            var template = actual[5];
            Assert.IsTrue(template.Name.Length > 0);
            Assert.IsTrue(template.TemplateId > 0);
}
    }
}
