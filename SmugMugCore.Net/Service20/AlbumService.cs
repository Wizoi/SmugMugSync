using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Xps.Serialization;
using SmugMug.Net;
//using SmugMug.Net.Data20.Domain.Album;

namespace SmugMug.Net.Service20
{
    public class AlbumService
    {
        private readonly Core20.SmugMugCore _core;

        public AlbumService(Core20.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Delete an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
/*
        public virtual async Task<bool> DeleteAlbum(int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.albums.delete", queryParams, false);
            return true;
        }
*/

        /// <summary>
        /// Retrieve a list of albums for a given user with a field list.  Will return empty albums, and ignore the nickname, and album passwords
        /// </summary>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
  /*      public async virtual Task<Data.AlbumDetail[]> GetAlbumList(string[] fieldList)
        {
            return await GetAlbumList(fieldList: fieldList, returnEmpty:true, nickName:string.Empty, sitePassword:string.Empty);
        }
*/

        /// <summary>
        /// Retrieve a list of albums for a given user
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
/*        public async Task<Data.AlbumDetail[]> GetAlbumList(string[] fieldList, bool returnEmpty, string nickName, string sitePassword)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Empty", returnEmpty, true);
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");  
            queryParams.Add("Heavy", false);
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.AlbumDetail>(fieldList) ?? "", "");

            var queryResponse = await _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.get", queryParams, true);

            // Return Results
            return queryResponse;
        }
*/
        /// <summary>
        /// Retrieve the information for an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <returns></returns>
   //     public async virtual Task<Data.AlbumDetail> GetAlbumDetail(int albumId, string albumKey)
   //     {
  //          return await GetAlbumDetailExt(albumId:albumId, albumKey:albumKey, albumPassword:"", sitePassword:"");
   //     }

        /// <summary>
        /// Retrieve the information for an album
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
    /*    public async Task<Data.AlbumDetail> GetAlbumDetailExt(int albumId, string albumKey, string albumPassword = "", string sitePassword = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var queryResponse = await _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.getInfo", queryParams, false);

            // Return Results
            return queryResponse[0];
        }
*/
        /// <summary>
        /// Retrieve a list of albums for a given user with info populated for each
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
  /*      public async Task<Data.AlbumDetail[]> GetAlbumDetailList(bool returnEmpty, string nickName, string sitePassword = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Empty", returnEmpty);
            queryParams.Add("NickName", nickName);
            queryParams.Add("SitePassword", sitePassword, "");

            queryParams.Add("Heavy", true);

            var queryResponse = await _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.get", queryParams, true);

            // Return Results
            return queryResponse;
        }
*/
 
        /// <summary>
        /// Create an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns>New Album Album object with Key Information</returns>
 /*       public async virtual Task<Data.AlbumDetail> CreateAlbum(Data.AlbumDetail album)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            AlbumService.AddAlbumParameters(queryParams, album);

            var response = await _core.QueryWebsite<Data.AlbumDetail>("smugmug.albums.create", queryParams, false);
            
            // Return a copy of the original object
            var newAlbum = response[0];
            var outAlbumDetail = (Data.AlbumDetail)album.Copy();
            outAlbumDetail.AlbumId = newAlbum.AlbumId;
            outAlbumDetail.AlbumKey = newAlbum.AlbumKey;
            return outAlbumDetail;
        }
*/
        /// <summary>
        /// Add the parameters in an album to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="album"></param>
/*        private static void AddAlbumParameters(Core.QueryParameterList queryParams, Data.AlbumDetail album)
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
*/
    }
}
