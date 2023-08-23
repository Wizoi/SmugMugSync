using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{

    [ObsoleteAttribute("smugmug.accounts.* no longer is working with v1.3.0 Smugmug API.")]
    public class AccountService
    {
        private readonly Core.SmugMugCore _core;

        public AccountService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Browse to an account
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public System.Uri Browse(string nickName, string sitePassword = "")
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName);
            queryParams.Add("SitePassword", sitePassword, "");

            var results = _core.QueryWebsite<System.Uri>("smugmug.accounts.browse", queryParams, false);
            return results[0];
        }
    }
}
