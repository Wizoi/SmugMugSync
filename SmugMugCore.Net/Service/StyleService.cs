using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.styles.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class StyleService
    {
        private readonly Core.SmugMugCore _core;

        public StyleService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Retrieve a list of sharegroups without the Album detail
        /// </summary>
        /// <returns></returns>
        public Data.Template[] GetTemplates()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = _core.QueryWebsite<Data.Template>("smugmug.styles.getTemplates", queryParams, true);

            // Return Results
            return queryResponse;
        }
    }
}
