using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [ObsoleteAttribute("smugmug.featured.albums.* no longer is working with v1.3.0 Smugmug API.")]
    public class FeaturedAlbumService
    {
        private readonly Core.SmugMugCore _core;

        public FeaturedAlbumService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Retrieve a list of featured albums for a given user
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public async Task<Data.AlbumDetail[]> GetFeaturedAlbumList(string[] fieldList, string nickName = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.FeaturedAlbums>(fieldList) ?? "", "");
            queryParams.Add("Heavy", false);

            var queryResponse = await _core.QueryWebsite<Data.FeaturedAlbums>("smugmug.featured.albums.get", queryParams, false);
            if (queryResponse != null && queryResponse.Length > 0)
            {
                var responseDetail = queryResponse[0];
                if (responseDetail != null && responseDetail.Albums != null)
                    return responseDetail.Albums;
            }

            return Array.Empty<Data.AlbumDetail>();
        }

        /// <summary>
        /// Retrieve a list of featured albums with full information for a given user
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public async Task<Data.AlbumDetail[]> GetFeaturedAlbumDetailList(string nickName = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");
            queryParams.Add("Heavy", true);

            var queryResponse = await _core.QueryWebsite<Data.FeaturedAlbums>("smugmug.featured.albums.get", queryParams, false);
            if (queryResponse != null && queryResponse.Length > 0)
            {
                var responseDetail = queryResponse[0];
                if (responseDetail != null && responseDetail.Albums != null)
                    return responseDetail.Albums;
            }

            // Return Results
            return Array.Empty<Data.AlbumDetail>();
        }
    }
}
