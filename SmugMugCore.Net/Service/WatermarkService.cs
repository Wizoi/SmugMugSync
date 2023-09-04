using System;
using System.Collections.Generic;
using SmugMug.Net;
using SmugMug.Net.Data.Domain.Watermark;

namespace SmugMug.Net.Service
{
    /// <summary>
    /// Service class to manage the watermarks for SmugMug
    /// </summary>
    [Obsolete("smugmug.watermarks.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class WatermarkService
    {
        private readonly Core.SmugMugCore _core;

        /// <summary>
        /// Constructor to create the watermark service
        /// </summary>
        /// <param name="core"></param>
        public WatermarkService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Retrieve a list of watermarks with optional album details
        /// </summary>
        /// <returns></returns>
        public async Task<Data.Watermark[]> GetWatermarkList(bool includeDetails, string[] fieldList)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.Comment>(fieldList) ?? "", ""); 
            queryParams.Add("Heavy", includeDetails);

            var queryResponse = await _core.QueryWebsite<Data.Watermark>("smugmug.watermarks.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve a list of album templates
        /// </summary>
        /// <returns></returns>
        public async Task<Data.Watermark> GetWatermark(int watermarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("WatermarkID", watermarkId);

            var queryResponse = await _core.QueryWebsite<Data.Watermark>("smugmug.watermarks.getInfo", queryParams, false);

            // Return Results
            return queryResponse[0];
        }


        /// <summary>
        /// Create a watermark
        /// Required: ImageID and a Name
        /// </summary>
        /// <returns></returns>
        public async Task<Data.Watermark> CreateWatermark(Data.Watermark watermark)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            AddWatermarkParameters(queryParams, watermark);

            var queryResponse = await _core.QueryWebsite<Data.Watermark>("smugmug.albumtemplates.create", queryParams, false);
            var respWatermark = queryResponse[0];
            var outWatermark = watermark.Copy();
            outWatermark.WatermarkId = respWatermark.WatermarkId;

            // Return Results
            return outWatermark;
        }


        /// <summary>
        /// Delete a watermark
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteWatermark(int watermarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("WatermarkID", watermarkId);

            _ = await _core.QueryWebsite<Data.AlbumTemplate>("smugmug.watermarks.delete", queryParams, false);

            // Return True if no error is thrown
            return true;
        }

        /// <summary>
        /// Change the settings of a watermark
        /// </summary>
        /// <param name="watermark">Watermark object to change settings on</param>
        /// <returns></returns>
        public async Task<bool> UpdateWatermark(Data.Watermark watermark)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("WatermarkID", watermark.WatermarkId);
            WatermarkService.AddWatermarkParameters(queryParams, watermark);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.watermarks.changeSettings", queryParams, false);
            return true;
        }

        /// <summary>
        /// Add the parameters in a watermark to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="album"></param>
        private static void AddWatermarkParameters(Core.QueryParameterList queryParams, Data.Watermark watermark)
        {
            queryParams.Add("Dissolve", watermark.Dissolve);
            if (watermark.Image != null)
                queryParams.Add("ImageID", watermark.Image.ImageId);
            if (watermark.Name != null)
                queryParams.Add("Name", watermark.Name);
            string? pinnedValue = Enum.GetName(typeof(Pinned), watermark.PinnedLocation);
            if (pinnedValue != null)
                queryParams.Add("Pinned", pinnedValue);
            if (watermark.ThumbnailWatermarkEnabled != null)
                queryParams.Add("Thumbs", watermark.ThumbnailWatermarkEnabled);
        }
    }
}
