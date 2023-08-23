namespace TestSmugMugCoreNetAPI
{
    public class Utility
    {

        /// <summary>
        /// Authenticate and provide a smug mug core to test against
        /// </summary>
        /// <returns></returns>
        public static SmugMug.Net.Core.SmugMugCore RetrieveSmugMugCore()
        {
            var settings = new Configuration.KeySecretsConfig();
            var manager = new SmugMug.Net.Core.SmugMugCore(
                userAuthToken:  settings.UserAuthToken, userAuthSecret: settings.UserAuthSecret,
                apiKey: settings.ApiKey, apiSecret: settings.ApiSecret);
            manager.EnableRequestLogging = true;
            return manager;
        }

        /// <summary>
        /// Create an album for testing
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static SmugMug.Net.Data.AlbumDetail CreateArbitraryTestAlbum(SmugMug.Net.Core.SmugMugCore core, string title)
        {
            // Remove it first if it exists from a prior run to clean up
            RemoveArbitraryTestAlbum(core, title);

            AlbumService service = new AlbumService(core); 
            var album = new AlbumDetail();
            album.Title = title;
            album.BackprintingForPrints = "";
            album.BoutiquePackagingForOrders = SmugMug.Net.Data.Domain.Album.BoutiquePackaging.No;
            album.CanRank = true;
            album.CleanDisplay = true;
            album.ColorCorrection = SmugMug.Net.Data.Domain.Album.ColorCorrection.No;
            album.CommentsAllowed = true;
            album.Description = "This is a test album to verify my API";
            album.ExifAllowed = true;
            album.ExternalLinkAllowed = true;
            album.FamilyEditAllowed = true;
            album.FilenameDisplayWhenNoCaptions = true;
            album.FriendEditAllowed = true;
            album.GeographyMappingEnabled = true;
            album.HeaderDefaultIsSmugMug = true;
            album.HideOwner = false;
            album.InterceptShippingEnabled = SmugMug.Net.Data.Domain.Album.InterceptShipping.No;
            album.Keywords = "test;test2";
            album.NiceName = title + "NiceName";
            album.PackageBrandedOrdersEnabled = true;
            album.PublicDisplay = true;
            var newAlbum = service.CreateAlbum(album);
            return newAlbum;
        }

        /// <summary>
        /// Remove an arbitrary test album
        /// </summary>
        /// <param name="core"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public static void RemoveArbitraryTestAlbum(SmugMug.Net.Core.SmugMugCore core, string title)
        {
            var service = new AlbumService(core);
            var albumData = service.GetAlbumList(Array.Empty<string>()).Where(x=>x.Title == title).ToArray();
            if (albumData.Length > 0)
                service.DeleteAlbum(albumData[0].AlbumId);
        }

        [Obsolete()]
        public static void RemoveArbitraryTestAlbumTemplates(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new AlbumTemplateService(core);
            var albumData = service.GetAlbumTemplateList().Where(x => x.AlbumTemplateName.Contains(name)).ToArray();
            foreach (var alb in albumData)
            {
                service.DeleteAlbumTemplate(alb.AlbumTemplateId);
            }
        }

        [Obsolete()]
        public static void RemoveArbitraryTestCategories(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new CategoryService(core);
            var data = service.GetCategoryList().Where(x => x.Name.Contains(name)).ToArray();
            foreach (var d in data)
            {
                if (d.Type != "SmugMug")
                {
                    service.DeleteCategory(d.CategoryId);
                }
            }
        }

        [Obsolete()]
        public static void RemoveArbitraryTestCoupons(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new CouponService(core);
            var data = service.GetCouponInfoList().Where(x => x.Title.Contains(name)).ToArray();
            foreach (var d in data)
            {
                //service.Delete(d.CategoryId);
            }
        }

        public static void RemoveArbitraryTestFamilies(SmugMug.Net.Core.SmugMugCore core, string nickname)
        {
            var service = new FamilyService(core);
            var data = service.GetFamilyList().Where(x => x.NickName.Contains(nickname)).ToArray();
            foreach (var d in data)
            {
                service.RemoveFamily(d.NickName);
            }
        }

        public static void RemoveArbitraryTestFriends(SmugMug.Net.Core.SmugMugCore core, string nickname)
        {
            var service = new FriendService(core);
            var data = service.GetFriendList().Where(x => x.NickName.Contains(nickname)).ToArray();
            foreach (var d in data)
            {
                service.RemoveFriend(d.NickName);
            }
        }

        [Obsolete()]
        public static void RemoveArbitraryTestPrintmarks(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new PrintmarkService(core);
            var data = service.GetPrintmarkList(Array.Empty<string>()).Where(x => x.Name.Contains(name)).ToArray();
            foreach (var d in data)
            {
                service.DeletePrintmark(d.PrintmarkId);
            }
        }

        [Obsolete()]
        public static void RemoveArbitraryTestSubCategories(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new SubCategoryService(core);
            var data = service.GetAllSubCategories().Where(x => x.Name.Contains(name)).ToArray();
            foreach (var d in data)
            {
                service.DeleteSubCategory(d.SubCategoryId);
            }
        }

        [Obsolete()]
        public static void RemoveArbitraryTestWatermarks(SmugMug.Net.Core.SmugMugCore core, string name)
        {
            var service = new WatermarkService(core);
            var data = service.GetWatermarkList(false, Array.Empty<string>()).Where(x => x.Name.Contains(name)).ToArray();
            foreach (var d in data)
            {
                service.DeleteWatermark(d.WatermarkId);
            } 
        }

    }
}
