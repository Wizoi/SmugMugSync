namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for AlbumTemplateServiceTest and is intended
    ///to contain all AlbumTemplateServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class AlbumTemplateServiceTest
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
        ///A test for CreateAlbumTemplate
        ///</summary>
        [TestMethod()]
        public void CreateDeleteAlbumTemplateTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            AlbumTemplateService target = new AlbumTemplateService(core); 
            AlbumTemplate expected = new AlbumTemplate();
            expected.AlbumTemplateName = "Test Template Create Name";
            
            // Make sure any prior test artifect is cleaned up
            Utility.RemoveArbitraryTestAlbumTemplates(core, expected.AlbumTemplateName);

            var actual = target.CreateAlbumTemplate(expected);
            Assert.IsTrue(actual.AlbumTemplateId > 0);

            // Clean up album template
            target.DeleteAlbumTemplate(actual.AlbumTemplateId);
        }

        /// <summary>
        ///A test for GetAlbumTemplateList
        ///</summary>
        [TestMethod()]
        public void GetAlbumTemplateListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            AlbumTemplateService target = new AlbumTemplateService(core);
            var actual = target.GetAlbumTemplateList();
            Assert.IsTrue(actual.Length > 0);
        }

        /// <summary>
        ///A test for UpdateAlbumTemplate
        ///</summary>
        [TestMethod()]
        public void UpdateAlbumTemplateTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            AlbumTemplateService target = new AlbumTemplateService(core);
            AlbumTemplate expected = new AlbumTemplate();
            expected.AlbumTemplateName = "Test Template Update Name";

            // Make sure any prior test artifect is cleaned up
            Utility.RemoveArbitraryTestAlbumTemplates(core, expected.AlbumTemplateName);
            
            var actual = target.CreateAlbumTemplate(expected);
            Assert.AreEqual(expected.AlbumTemplateName, actual.AlbumTemplateName);

            actual.AlbumTemplateName = "Test Updated Template Update Name";
            if (target.UpdateAlbumTemplate(actual))
            {

                // Wait and Get the subcategory
                int count = 0;
                AlbumTemplate tgtSearch = null;
                do
                {
                    var list = target.GetAlbumTemplateList();
                    var found = list.Where(x => x.AlbumTemplateName == actual.AlbumTemplateName);
                    if (found.Count() > 0)
                    {
                        tgtSearch = found.Single();
                        count = 5;
                    }
                    else
                    {
                        count++;
                        System.Threading.Thread.Sleep(1000);
                    }

                } while (count < 5);
                Assert.IsNotNull(tgtSearch);
            }

            // Clean up album template
            target.DeleteAlbumTemplate(actual.AlbumTemplateId);
        }
    }
}
