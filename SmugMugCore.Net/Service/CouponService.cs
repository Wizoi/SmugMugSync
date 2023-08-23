using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [ObsoleteAttribute("smugmug.coupons.* is not testable to validate still working with v1.3.0 Smugmug API.")]
    public class CouponService
    {
        private readonly Core.SmugMugCore _core;

        public CouponService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of coupon IDs
        /// </summary>
        /// <param name="status">The comma separated string of status values to filter results</param>
        /// <param name="type">The comma separated string of type values to filter results</param>
        /// <returns></returns>
        public Data.CouponCore[] GetCouponList(string[] fieldList, string statusFilter = "", string typeFilter = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Status", statusFilter, "");
            queryParams.Add("Type", typeFilter, "");
            queryParams.Add("Heavy", false);
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.CouponCore>(fieldList) ?? "", "");

            var queryResponse = _core.QueryWebsite<Data.CouponCore>("smugmug.coupons.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve a list of coupons with full information
        /// </summary>
        /// <param name="status">The comma separated string of status values to filter results</param>
        /// <param name="type">The comma separated string of type values to filter results</param>
        /// <returns></returns>
        public Data.CouponInfo[] GetCouponInfoList(string statusFilter = "", string typeFilter = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Status", statusFilter);
            queryParams.Add("Type", typeFilter);
            queryParams.Add("Heavy", true);

            var queryResponse = _core.QueryWebsite<Data.CouponInfo>("smugmug.coupons.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve information for a coupon
        /// </summary>
        /// <param name="couponId">The id for a specific coupon</param>
        /// <returns></returns>
        public Data.CouponInfo GetCoupon(string couponId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CouponID", couponId);

            var queryResponse = _core.QueryWebsite<Data.CouponInfo>("smugmug.coupons.getInfo", queryParams, false);


            // Return Results
            return queryResponse[0];
        }

        /// <summary>
        /// Add an album restriction to a coupon
        /// </summary>
        /// <param name="couponId">The id for a specific coupon</param>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        public bool AddAlbumRestrictionToCoupon(string couponId, string albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CouponID", couponId);
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.CouponInfo>("smugmug.coupons.restrictions.albums.add", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Remove an album restriction to a coupon
        /// </summary>
        /// <param name="couponId">The id for a specific coupon</param>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns></returns>
        public bool RemoveAlbumRestrictionFromCoupon(string couponId, string albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CouponID", couponId);
            queryParams.Add("AlbumID", albumId);

            _core.QueryWebsite<Data.CouponInfo>("smugmug.coupons.restrictions.albums.remove", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Create a coupon 
        /// Required Values in a coupon are: Code, TItle & Type
        /// </summary>
        /// <param name="coupon">Coupon object to create</param>
        /// <returns>New coupon object with Key Information</returns>
        public Data.CouponInfo CreateCoupon(Data.CouponInfo coupon)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            CouponService.AddCouponParameters(queryParams, coupon);

            var response = _core.QueryWebsite<Data.CouponCore>("smugmug.coupons.create", queryParams, false);

            // Return a copy of the original object
            var newCoupon = response[0];
            var outCouponInfo = (Data.CouponInfo)coupon.Copy();
            outCouponInfo.CouponId = newCoupon.CouponId;
            return outCouponInfo;
        }


        /// <summary>
        /// Change the settings of a coupon 
        /// </summary>
        /// <param name="coupon">Coupon object to change settings on</param>
        /// <returns></returns>
        public bool UpdateCoupon(Data.CouponInfo coupon)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CouponID", coupon.CouponId);
            CouponService.AddCouponParameters(queryParams, coupon);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.coupons.modify", queryParams, false);
            return true;
        }

        /// <summary>
        /// Add the parameters in a coupon to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="coupon"></param>
        private static void AddCouponParameters(Core.QueryParameterList queryParams, Data.CouponInfo coupon)
        {
            if (coupon.Code != null)
                queryParams.Add("Code", coupon.Code);
            if (coupon.Description != null)
                queryParams.Add("Description", coupon.Description);
            queryParams.Add("Discount", coupon.Discount);
            queryParams.Add("International", coupon.InternationalShippingCovered);
            queryParams.Add("MaxDiscount", coupon.MaxDiscount);
            queryParams.Add("MaxUses", coupon.MaxUses);
            queryParams.Add("MinPurchase", coupon.MinPurchase);
            queryParams.Add("Shipping", coupon.ShippingCovered);
            if (coupon.Title != null)
                queryParams.Add("Title", coupon.Title);
            if (coupon.ValidFromDate != null)
                queryParams.Add("ValidFrom", coupon.ValidFromDate);
            if (coupon.ValidToDate != null)
                queryParams.Add("ValidTo", coupon.ValidToDate);

            // Add the restrictions to an AlbumIDs Array for the query
            if (coupon.Restrictions != null)
            {
                var albumIdData = from c in coupon.Restrictions
                                  select c.AlbumId;
                var albumIdArray = albumIdData.ToArray();
                var albumIdList = string.Join(",", albumIdArray);
                queryParams.Add("AlbumIDs", albumIdList);
            }
            else
            {
                queryParams.Add("AlbumIDs", "");
            }
        }
    }
}
