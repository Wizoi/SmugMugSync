namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for CategoryServiceTest and is intended
    ///to contain all CategoryServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class CategoryServiceTest
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
        ///A test for GetCategoryList
        ///</summary>
        [TestMethod()]
        public void GetCategoryListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new CategoryService(core); 
            var actual = target.GetCategoryList();
            Assert.IsTrue(actual.Length > 0);
        }

        /// <summary>
        ///A test for CreateCategory
        ///</summary>
        [TestMethod()]
        public void CreateDeleteCategoryTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new CategoryService(core); 
            string name = "Test Category Creation";
            string niceName = "TestCategoryCreation";

            // Clean up prior failed attempts
            Utility.RemoveArbitraryTestCategories(core, name);
            
            var actual = target.CreateCategory(name, niceName);
            Assert.AreEqual(actual.Name, name);
            Assert.AreEqual(actual.NiceName, niceName);
            Assert.IsTrue(actual.CategoryId > 0);

            Utility.RemoveArbitraryTestCategories(core, name);
        }

        /// <summary>
        ///A test for RenameCategory
        ///</summary>
        [TestMethod()]
        public void RenameCategoryTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new CategoryService(core);
            string name = "Test Category Rename";
            string niceName = "TestCategoryRename";

            // Clean up prior failed attempts
            Utility.RemoveArbitraryTestCategories(core, name);

            var actual = target.CreateCategory(name, niceName);

            Assert.AreEqual(actual.Name, name);
            Assert.AreEqual(actual.NiceName, niceName);
            Assert.IsTrue(actual.CategoryId > 0);

            target.RenameCategory(actual.CategoryId, "Test Category Rename - New Name");
            var categoryList = target.GetCategoryList();
            var category = categoryList.Where(x => x.Name == "Test Category Rename - New Name");
            Assert.AreEqual(category.Count(), 0);

            
            Utility.RemoveArbitraryTestCategories(core, name);
        }
    }
}
