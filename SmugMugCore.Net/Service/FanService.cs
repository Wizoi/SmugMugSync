using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    public class FanService
    {
        private readonly Core.SmugMugCore _core;

        public FanService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of fans for a user
        /// </summary>
        /// <returns></returns>
        public  async Task<Data.Fan[]> GetFanList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = await _core.QueryWebsite<Data.Fan>("smugmug.fans.get", queryParams, true);

            // Return Results
            return queryResponse;
        }


    }
}
