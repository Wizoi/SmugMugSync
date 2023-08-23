using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.sharegroups.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class ShareGroupService
    {
        private readonly Core.SmugMugCore _core;

        public ShareGroupService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Add an album to a share group
        /// </summary>
        /// <param name="shareGroupId">The id for a specific share group</param>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        public bool AddAlbumToShareGroup(string shareGroupId, string albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ShareGroupID", shareGroupId);
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.CouponInfo>("smugmug.sharegroups.albums.add", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Remove an album restriction to a coupon
        /// </summary>
        /// <param name="couponId">The id for a specific coupon</param>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        public bool RemoveAlbumFromShareGroup(string shareGroupId, string albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ShareGroupID", shareGroupId);
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.CouponInfo>("smugmug.sharegroups.albums.remove", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Remove the albums for a sharegroup
        /// </summary>
        /// <param name="shareGroupTag">The tag (public id) for the sharegroup</param>
        /// <param name="password">The password for the sharegroup</param>
        /// <param name="fieldList">Extra Album fields to return in results</param>
        /// <returns></returns>
        public Data.AlbumDetail[] GetShareGroupAlbums(string[] fieldList, string shareGroupTag, string password = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ShareGroupTag", shareGroupTag);
            queryParams.Add("Password", password, "");
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.AlbumDetail>(fieldList) ?? "", "");

            var response = _core.QueryWebsite<Data.ShareGroup>("smugmug.sharegroups.albums.get", queryParams, true);
            if (response != null && response.Length > 0)
            {
                var responseDetail = response[0];
                if (responseDetail != null && responseDetail.Albums != null)
                    return responseDetail.Albums;
            }

            // Return Results
            return Array.Empty<Data.AlbumDetail>();
        }

        /// <summary>
        /// Browse to a sharegroup
        /// </summary>
        /// <param name="shareGroupTag">The tag (public id) for the sharegroup</param>
        /// <param name="password">The password for the sharegroup</param>
        /// <returns></returns>
        public System.Uri Browse(string shareGroupTag, string password = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ShareGroupTag", shareGroupTag);
            queryParams.Add("Password", password, "");

            var results = _core.QueryWebsite<System.Uri>("smugmug.sharegroups.browse", queryParams, false);
            return results[0];
        }

        /// <summary>
        /// Create a sharegroup
        /// Requred Fields: Name
        /// </summary>
        /// <param name="shareGroup">Sharegroup data to add</param>
        /// <returns></returns>
        public Data.ShareGroup CreateSharegroup(Data.ShareGroup shareGroup)
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            ShareGroupService.AddShareGroupParameters(queryParams, shareGroup);

            var response = _core.QueryWebsite<Data.ShareGroup>("smugmug.sharegroups.create", queryParams, false);
            // Return a copy of the original object
            var newShareGroup = response[0];
            var outShareGroup = (Data.ShareGroup)shareGroup.Copy();
            outShareGroup.ShareGroupId = newShareGroup.ShareGroupId;
            return outShareGroup;
        }

        /// <summary>
        /// Change the settings of a ShareGroup 
        /// </summary>
        /// <param name="coupon">Printmark object to change settings on</param>
        /// <returns></returns>
        public bool UpdateShareGroup(Data.ShareGroup shareGroup)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ShareGroupID", shareGroup.ShareGroupId);
            ShareGroupService.AddShareGroupParameters(queryParams, shareGroup);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.sharegroups.modify", queryParams, false);
            return true;
        }


        /// <summary>
        /// Retrieve a list of sharegroups without the Album detail
        /// </summary>
        /// <returns></returns>
        public Data.ShareGroup[] GetShareGroupList(bool includeAlbums = false)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Heavy", includeAlbums);

            var queryResponse = _core.QueryWebsite<Data.ShareGroup>("smugmug.sharegroups.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Add the parameters in a sharegorup to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="coupon"></param>
        private static void AddShareGroupParameters(Core.QueryParameterList queryParams, Data.ShareGroup shareGroup)
        {
            queryParams.Add("AccessPassworded", shareGroup.AccessPassworded);
            if (shareGroup.Description != null)
                queryParams.Add("Description", shareGroup.Description);
            if (shareGroup.Name != null)
                queryParams.Add("Name", shareGroup.Name);
            if (shareGroup.Password != null)
                queryParams.Add("Password", shareGroup.Password);
            if (shareGroup.PasswordHint != null)
                queryParams.Add("PasswordHint", shareGroup.PasswordHint);
        }


    }
}
