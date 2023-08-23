using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.users.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class UserService
    {
        private readonly Core.SmugMugCore _core;

        public UserService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Retrieve a list of sharegroups without the Album detail
        /// </summary>
        /// <returns></returns>
        public Data.User GetUser(string nickname)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickname);

            var queryResponse = _core.QueryWebsite<Data.User>("smugmug.users.getInfo", queryParams, false);

            // Return Results
            return queryResponse[0];
        }

        /// <summary>
        /// Retrieve the statistics for a user
        /// </summary>
        /// <param name="month">The month to retrieve statistics for</param>
        /// <param name="year">The year to retrieve statistics for</param>
        /// <returns></returns>
        public Data.UserStats GetUserStats(int month, int year, bool includeAlbums)
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Month", month);
            queryParams.Add("Year", year);

            queryParams.Add("Heavy", includeAlbums);

            var queryResponse = _core.QueryWebsite<Data.UserStats>("smugmug.users.getStats", queryParams, false);

            // Return Results
            return queryResponse[0];
        }

        /// <summary>
        /// Retrieve a list of albums for a given user
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="shareGroupTag">The tag (public id) for the sharegroup</param>
        /// <returns></returns>
        public Data.UserTreeCategory[] GetUserTree(
            bool returnAlbumDetail = false, bool returnEmpty = true, string nickname = "", string sitePassword = "", string shareGroupTag = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Empty", returnEmpty);
            queryParams.Add("NickName", nickname);
            queryParams.Add("SitePassword", sitePassword, "");
            queryParams.Add("ShareGroupTag", shareGroupTag, "");
            queryParams.Add("Heavy", returnAlbumDetail);

            var queryResponse = _core.QueryWebsite<Data.UserTreeCategory>("smugmug.users.getTree", queryParams, true);

            // Return Results
            return queryResponse;
        }

    }
}
