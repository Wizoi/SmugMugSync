using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    public class CommunityService
    {
        private Core.SmugMugCore _core;

        public CommunityService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of communities for a user
        /// </summary>
        /// <returns></returns>
        public async Task<Data.Community[]> GetCommunityList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = await _core.QueryWebsite<Data.Community>("smugmug.communities.get", queryParams, true);

            // Return Results
            return queryResponse;
        }
    }
}
