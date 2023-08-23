using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;
using SmugMug.Net.Data.Domain.Album;

namespace SmugMug.Net.Service
{
    public class AlbumService
    {
        private readonly Core.SmugMugCore _core;

        public AlbumService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Applies a watermark to the images of an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="watermarkId">The id for a specific watermark</param>
        /// <returns></returns>
        [Obsolete("ApplyWatermark Method is no longer vaid")]
        public bool ApplyWatermark(int albumId, int watermarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("WatermarkID", watermarkId);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.albums.applyWatermark", queryParams, false);
            return true;
        }

        /// <summary>
        /// Remove a watermark from the images of an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.removeWatermark no longer is working with v1.3.0 Smugmug API.")]
        public bool RemoveWatermark(int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.albums.removeWatermark", queryParams, false);
            return true;
        }

        /// <summary>
        /// Resort an album according to album defaults
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.reSort no longer is working with v1.3.0 Smugmug API.")]
        public bool Resort(int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.albums.reSort", queryParams, false);
            return true;
        }

        /// <summary>
        /// Browse to an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="password">The password for the album</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.browse no longer is working with v1.3.0 Smugmug API.")]
        public System.Uri Browse(int albumId, string albumKey, string password = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Password", password, "");

            var results = _core.QueryWebsite<System.Uri>("smugmug.albums.browse", queryParams, false);
            return results[0];
        }

        /// <summary>
        /// Add a comment to an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="comment">The text for the comment</param>
        /// <param name="rating">The rating for the comment. 
        /// Values: 0 - No Rating (default), 1 - 1 Star Rating, 2 - 2 Star Rating, 3 - 3 Star Rating, 4 - 4 Star Rating, 5 - 5 Star Rating</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.comments.add no longer is working with v1.3.0 Smugmug API.")]
        public Data.Comment AddComment(int albumId, string albumKey, string[] fieldList, string comment, int rating = 0)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Text", comment);
            queryParams.Add("Rating", rating, 0);
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.Comment>(fieldList) ?? "", "");

            var response = _core.QueryWebsite<Data.Comment>("smugmug.albums.comments.add", queryParams, false);
            var retVal = response[0];
            retVal.Text = comment;
            retVal.Rating = rating;
            return retVal;
        }


        /// <summary>
        /// Retrieve a list of comments for an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.comments.get no longer is working with v1.3.0 Smugmug API.")]
        public Data.Comment[] GetCommentList(int albumId, string albumKey, string albumPassword = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            Data.AlbumDetail[]? response = _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.comments.get", queryParams, false);
            if (response != null && response.Length > 0)
            {
                var responseDetail = response[0];
                if (responseDetail != null && responseDetail.Comments != null)
                    return responseDetail.Comments;
            }
            return Array.Empty<Data.Comment>(); 
        }

        /// <summary>
        /// Delete an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        public bool DeleteAlbum(int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.albums.delete", queryParams, false);
            return true;
        }


        /// <summary>
        /// Retrieve a list of albums for a given user
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
        public Data.AlbumDetail[] GetAlbumList(string[] fieldList, bool returnEmpty = true, string nickName = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Empty", returnEmpty, true);
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");  
            queryParams.Add("Heavy", false);
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.AlbumDetail>(fieldList) ?? "", "");

            var queryResponse = _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve the information for an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public Data.AlbumDetail GetAlbumDetail(int albumId, string albumKey, string albumPassword = "", string sitePassword = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var queryResponse = _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.getInfo", queryParams, false);

            // Return Results
            return queryResponse[0];
        }

        /// <summary>
        /// Retrieve a list of albums for a given user with info populated for each
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public Data.AlbumDetail[] GetAlbumDetailList(bool returnEmpty, string nickName, string sitePassword = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Empty", returnEmpty);
            queryParams.Add("NickName", nickName);
            queryParams.Add("SitePassword", sitePassword, "");

            queryParams.Add("Heavy", true);

            var queryResponse = _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve the statistics for an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="month">The month to retrieve statistics for</param>
        /// <param name="year">The year to retrieve statistics for</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.getStats no longer is working with v1.3.0 Smugmug API.")]
        public Data.AlbumStats GetAlbumStats(int albumId, int month, int year, bool includImageInfo = false)
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("Month", month);
            queryParams.Add("Year", year);

            queryParams.Add("Heavy", includImageInfo);

            var queryResponse = _core.QueryWebsite<Data.AlbumStats>("smugmug.albums.getStats", queryParams, false);
            var albumStats = queryResponse[0];
            albumStats.AlbumId = albumId;
            albumStats.Month = month;
            albumStats.Year = year;

            // Return Results
            return albumStats;
        }

        /// <summary>
        /// Change the settings of an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.albums.changeSettings no longer is working with v1.3.0 Smugmug API.")]
        public bool UpdateAlbum(Data.AlbumDetail album)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", album.AlbumId);
            AlbumService.AddAlbumParameters(queryParams, album);

            _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.changeSettings", queryParams, false);
            return true;
        }

        /// <summary>
        /// Create an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns>New Album Album object with Key Information</returns>
        public Data.AlbumDetail CreateAlbum(Data.AlbumDetail album)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            AlbumService.AddAlbumParameters(queryParams, album);

            var response = _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.create", queryParams, false);
            
            // Return a copy of the original object
            var newAlbum = response[0];
            var outAlbumDetail = (Data.AlbumDetail)album.Copy();
            outAlbumDetail.AlbumId = newAlbum.AlbumId;
            outAlbumDetail.AlbumKey = newAlbum.AlbumKey;
            return outAlbumDetail;
        }

        /// <summary>
        /// Add the parameters in an album to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="album"></param>
        private static void AddAlbumParameters(Core.QueryParameterList queryParams, Data.AlbumDetail album)
        {
            if ((album.NiceName != null) && !Core.SmugMugCore.IsValidNiceName(album.NiceName))
                throw new ApplicationException("A Nicename is defined by RFC 952 and RFC 1123 and is used like a hostname and is currently INVALID.");

            // Required Parameters
            if (album.Title != null)
                queryParams.Add("Title", album.Title);

            if (album.Template != null)
                queryParams.Add("AlbumTemplateID", album.Template.TemplateId);
            else
               queryParams.Add("AlbumTemplateID", "");
            if (album.BackprintingForPrints != null)
                queryParams.Add("Backprinting", album.BackprintingForPrints);
            queryParams.Add("BoutiquePackaging", (int) album.BoutiquePackagingForOrders);
            queryParams.Add("CanRank", album.CanRank);
            if (album.Category != null)
                queryParams.Add("CategoryID", album.Category.CategoryId);
            else
                queryParams.Add("CategoryID", "");
            queryParams.Add("Clean", album.CleanDisplay);
            queryParams.Add("ColorCorrection", (int) album.ColorCorrection);
            queryParams.Add("Comments", album.CommentsAllowed);
            if (album.Community != null)
                queryParams.Add("CommunityID", album.Community.CommunityId);
            else
                queryParams.Add("CommunityID", "");
            if (album.Description != null)
                queryParams.Add("Description", album.Description);
            else
                queryParams.Add("Description", string.Empty);
            queryParams.Add("EXIF", album.ExifAllowed);
            queryParams.Add("External", album.ExternalLinkAllowed);
            queryParams.Add("FamilyEdit", album.FamilyEditAllowed);
            queryParams.Add("Filenames", album.FilenameDisplayWhenNoCaptions);
            queryParams.Add("FriendEdit", album.FriendEditAllowed);
            queryParams.Add("Geography", album.GeographyMappingEnabled);
            queryParams.Add("Header", album.HeaderDefaultIsSmugMug);
            queryParams.Add("HideOwner", album.HideOwner);
            if (album.Highlight != null)
                queryParams.Add("HighlightID", album.Highlight.HighlightImageId);
            else
                queryParams.Add("HighlightID", "");
            if (album.Keywords != null)
                queryParams.Add("Keywords", album.Keywords);
            else
                queryParams.Add("Keywords", string.Empty);
            queryParams.Add("InterceptShipping", (int) album.InterceptShippingEnabled);
            queryParams.Add("Larges", album.ViewingLargeImagesEnabled);
            if (album.NiceName != null)
                queryParams.Add("NiceName", album.NiceName);
            queryParams.Add("Originals", album.ViewingOriginalImagesEnabled);
            queryParams.Add("PackagingBranding", album.PackageBrandedOrdersEnabled);
            if (album.Password != null)
                queryParams.Add("Password", album.Password);
            if (album.PasswordHint != null)
                queryParams.Add("PasswordHint", album.PasswordHint);
            queryParams.Add("Position", album.Position);
            queryParams.Add("Printable", album.PurchaseEnabled);
            if (album.PrintmarkApplied != null)
                queryParams.Add("PrintMarkID", album.PrintmarkApplied.PrintmarkId);
            else
                queryParams.Add("PrintMarkID", "");
            queryParams.Add("ProofDays", album.ProofHoldDays);
            queryParams.Add("Protected", album.ProtectionEnabled);
            queryParams.Add("Public", album.PublicDisplay);
            queryParams.Add("Share", album.ShareEnabled);
            queryParams.Add("SmugSearchable", album.SmugSearchEnabled);
            queryParams.Add("SortDirection", album.SortDirectionDescending);
            string? sortMethodValue = System.Enum.GetName(typeof(SortMethod), album.SortMethod);
            if (sortMethodValue != null)
                queryParams.Add("SortMethod", sortMethodValue);
            queryParams.Add("SquareThumbs", album.SquareThumbnailCropEnabled);
            if (album.SubCategory != null)
                queryParams.Add("SubCategoryID", album.SubCategory.SubCategoryId);
            else
                queryParams.Add("SubCategoryID", "");
            if (album.Template != null)
                queryParams.Add("TemplateID", album.Template.TemplateId);
            else
                queryParams.Add("TemplateID", "");
            if (album.Theme != null)
                queryParams.Add("ThemeID", album.Theme.ThemeId);
            else
                queryParams.Add("ThemeID", "");
            queryParams.Add("UnsharpAmount", album.UnsharpAmount);
            queryParams.Add("UnsharpRadius", album.UnsharpRadius);
            queryParams.Add("UnsharpSigma", album.UnsharpSigma);
            queryParams.Add("UnsharpThreshold", album.UnsharpThreshold);
            if (album.Watermark != null)
                queryParams.Add("WatermarkID", album.Watermark.WatermarkId);
            else
                queryParams.Add("WatermarkID", "");
            queryParams.Add("WorldSearchable", album.WorldSearchableAllowed);
            queryParams.Add("X2Larges", album.ViewingLargeX2ImagesEnabled);
            queryParams.Add("X3Larges", album.ViewingLargeX3ImagesEnabled);
        }

    }
}
