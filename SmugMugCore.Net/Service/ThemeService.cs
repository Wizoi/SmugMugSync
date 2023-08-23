using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.themese.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class ThemeService
    {
        private readonly Core.SmugMugCore _core;

        public ThemeService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of sharegroups without the Album detail
        /// </summary>
        /// <returns></returns>
        public Data.Theme[] GetThemes()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = _core.QueryWebsite<Data.Theme>("smugmug.themes.get", queryParams, true);

            // Return Results
            return queryResponse;
        }
    }
}
