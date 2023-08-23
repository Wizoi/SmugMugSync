namespace TestSmugMugCoreNetAPI
{


    /// <summary>
    ///This is a test class for SubCategoryServiceTest and is intended
    ///to contain all SubCategoryServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class SubCategoryServiceTest
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
        ///A test for CreateSubCategory
        ///</summary>
        [TestMethod()]
        public void CreateDeleteSubCategoryTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new SubCategoryService(core);
            string name = "Test SubCategory Creation";

            // Clean up prior failed attempts
            Utility.RemoveArbitraryTestSubCategories(core, name);

            var actual = target.CreateSubCategory(0, name);
            Assert.AreEqual(actual.Name, name);
            Assert.IsTrue(actual.SubCategoryId > 0);

            Utility.RemoveArbitraryTestSubCategories(core, name);
        }


        /// <summary>
        ///A test for GetCategoryList
        ///</summary>
        [TestMethod()]
        public void GetAllSubCategoriesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new SubCategoryService(core);

            // Clean up prior failed attempts and create a stub subcategory
            string name = "Test SubCategory GetAll";
            Utility.RemoveArbitraryTestSubCategories(core, name);
            target.CreateSubCategory(0, name);
            
            // Wait and Get the subcategory
            int count = 0;
            SubCategory subCat = null;
            do
            {
                var list = target.GetAllSubCategories();
                var found = list.Where(x => x.Name.Contains(name));
                if (found.Count() > 0)
                {
                    subCat = found.Single();
                    Assert.IsNotNull(subCat);
                    Assert.AreEqual(subCat.Name, name);
                    count = 5;
                }
                else
                {
                    count++;
                    System.Threading.Thread.Sleep(1000);
                }

            } while (count < 5);
            Assert.IsNotNull(subCat);


            // Cleanup Sub Categories
            Utility.RemoveArbitraryTestSubCategories(core, name);

        }

        /// <summary>
        ///A test for GetCategoryList
        ///</summary>
        [TestMethod()]
        public void GetSubCategoryTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new SubCategoryService(core);

            // Clean up prior failed attempts and create a stub subcategory
            string name = "Test SubCategory GetOne";
            Utility.RemoveArbitraryTestSubCategories(core, name);
            target.CreateSubCategory(0, name);

            // Wait and Get the subcategory
            int count = 0;
            SubCategory subCat = null;
            do
            {
                var list = target.GetSubCategoryList(0);
                var found = list.Where(x => x.Name.Contains(name));
                if (found.Count() > 0)
                {
                    subCat = found.Single();
                    Assert.IsNotNull(subCat);
                    Assert.AreEqual(subCat.Name, name);
                    count = 5;
                }
                else
                {
                    count++;
                    System.Threading.Thread.Sleep(1000);
                }

            } while (count < 5);
            Assert.IsNotNull(subCat);


            // Cleanup Sub Categories
            Utility.RemoveArbitraryTestSubCategories(core, name);
        
        }


        /// <summary>
        ///A test for RenameSubCategory
        ///</summary>
        [TestMethod()]
        public void RenameSubCategoryTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new SubCategoryService(core);
            string name = "Test SubCategory Rename";

            // Clean up prior failed attempts
            Utility.RemoveArbitraryTestSubCategories(core, name);

            var actual = target.CreateSubCategory(0, name);

            Assert.AreEqual(actual.Name, name);

            // Change the name
            name = "Test SubCategory Rename - New Name";
            target.RenameSubCategory(actual.SubCategoryId, name);
            
            // Wait and Get the subcategory
            int count = 0;
            SubCategory subCat = null;
            do
            {
                var list = target.GetSubCategoryList(0);
                var found = list.Where(x => x.Name.Contains(name));
                if (found.Count() > 0)
                {
                    subCat = found.Single();
                    Assert.IsNotNull(subCat);
                    Assert.AreEqual(subCat.Name, name);
                    count = 5;
                }
                else
                {
                    count++;
                    System.Threading.Thread.Sleep(1000);
                }

            } while (count < 5);
            Assert.IsNotNull(subCat);

            Utility.RemoveArbitraryTestSubCategories(core, name);
        }
    }
}
