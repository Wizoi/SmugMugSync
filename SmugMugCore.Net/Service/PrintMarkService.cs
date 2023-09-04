using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using SmugMug.Net;
using SmugMug.Net.Data.Domain.Printmark;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.printmarks.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class PrintmarkService
    {
        private readonly Core.SmugMugCore _core;

        public PrintmarkService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of printmarks
        /// </summary>
        /// <returns></returns>
        public async Task<Data.PrintmarkInfo[]> GetPrintmarkList(string[] fieldList)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.PrintmarkInfo>(fieldList) ?? "", ""); 
            queryParams.Add("Heavy", false);

            var queryResponse = await _core.QueryWebsite<Data.PrintmarkInfo>("smugmug.printmarks.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve a list of printmarks with full information
        /// </summary>
        /// <returns></returns>
        public async Task<Data.PrintmarkInfo[]> GetPrintmarkInfoList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Heavy", true);

            var queryResponse = await _core.QueryWebsite<Data.PrintmarkInfo>("smugmug.printmarks.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve information for a printmark
        /// </summary>
        /// <param name="printmarkId">The id for a specific printmark</param>
        /// <returns></returns>
        public async Task<Data.PrintmarkInfo> GetPrintmark(string printmarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("PrintmarkID", printmarkId);

            var queryResponse = await _core.QueryWebsite<Data.PrintmarkInfo>("smugmug.coupons.getInfo", queryParams, false);


            // Return Results
            return queryResponse[0];
        }

        /// <summary>
        /// Delete a printmark 
        /// </summary>
        /// <param name="printmarkId">Printmark ID to delete</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeletePrintmark(int printmarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("PrintmarkID", printmarkId);

            _ = await _core.QueryWebsite<Data.CouponCore>("smugmug.coupons.delete", queryParams, false);
            return true;
        }


        /// <summary>
        /// Create a printmark
        /// Required Values in a printmark are: ImageID & Name
        /// </summary>
        /// <param name="coupon">Printmark object to create</param>
        /// <returns>Created Printmark</returns>
        public async Task<Data.PrintmarkInfo> CreatePrintmark(Data.PrintmarkInfo printmark)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            PrintmarkService.AddPrintmarkParameters(queryParams, printmark);

            var response = await _core.QueryWebsite<Data.PrintmarkInfo>("smugmug.printmark.create", queryParams, false);
            // Return a copy of the original object
            var newPrintmark = response[0];
            var outPrintmarkInfo = (Data.PrintmarkInfo) printmark.Copy();
            outPrintmarkInfo.PrintmarkId = newPrintmark.PrintmarkId;
            return outPrintmarkInfo;

        }


        /// <summary>
        /// Change the settings of a printmark 
        /// </summary>
        /// <param name="coupon">Printmark object to change settings on</param>
        /// <returns></returns>
        public async Task<bool> UpdatePrintmark(Data.PrintmarkInfo printmark)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("PrintmarkID", printmark.PrintmarkId);
            PrintmarkService.AddPrintmarkParameters(queryParams, printmark);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.printmark.modify", queryParams, false);
            return true;
        }

        /// <summary>
        /// Add the parameters in a printmark to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="coupon"></param>
        private static void AddPrintmarkParameters(Core.QueryParameterList queryParams, Data.PrintmarkInfo printmark)
        {
            queryParams.Add("Dissolve", printmark.Dissolve);
            if (printmark.Image != null && printmark.Image.ImageId != 0)
                queryParams.Add("ImageID", printmark.Image.ImageId);
            else
                queryParams.Add("ImageID", "");
            string? printmarkName = Enum.GetName(typeof(Location), printmark.Location);
            if (printmarkName != null)
                queryParams.Add("Location", printmarkName);
            if (printmark.Name != null)
                queryParams.Add("Name", printmark.Name);
        }

    }
}
