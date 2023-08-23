using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    public class FamilyService
    {
        private Core.SmugMugCore _core;

        public FamilyService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of family for a user
        /// </summary>
        /// <returns></returns>
        public Data.Family[] GetFamilyList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = _core.QueryWebsite<Data.Family>("smugmug.family.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Add a user to a user's list of family
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <returns></returns>
        public bool AddFamily(string nickName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.family.add", queryParams, false);

            return true;
        }

        /// <summary>
        /// Remove a user from a user's list of family
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <returns></returns>
        public bool RemoveFamily(string nickName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.family.remove", queryParams, false);

            return true;
        }

        /// <summary>
        /// Remove all users from a user's list of family
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            _core.QueryWebsite<Data.SmugmugError>("smugmug.family.removeAll", queryParams, false);

            return true;
        }


    }
}
