using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [Obsolete("smugmug.subcategories.* deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    public class SubCategoryService
    {
        private readonly Core.SmugMugCore _core;

        public SubCategoryService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of subcategories for a category
        /// </summary>
        /// <returns></returns>
        public async Task<Data.SubCategory[]> GetSubCategoryList(int categoryId, string nickName = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CategoryID", categoryId);
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var queryResponse = await _core.QueryWebsite<Data.SubCategory>("smugmug.subcategories.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Retrieve all subcategories for a category
        /// </summary>
        /// <returns></returns>
        public async Task<Data.SubCategory[]> GetAllSubCategories(string nickName = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var queryResponse = await _core.QueryWebsite<Data.SubCategory>("smugmug.subcategories.getAll", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Create a category
        /// </summary>
        /// <param name="categoryId">The id for a specific category</param>
        /// <param name="name">The name for the subcategory</param>
        /// <param name="unique">Create a subcategory if one of the same name doesn't already exist in the current hierarchy</param>
        /// <returns></returns>
        public async  Task<Data.SubCategory> CreateSubCategory(int categoryId, string name, bool unique = false)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CategoryID", categoryId);
            queryParams.Add("Name", name);
            queryParams.Add("Unique", unique, false);

            var queryResponse = await _core.QueryWebsite<Data.SubCategory>("smugmug.subcategories.create", queryParams, false);
            var subCategory = queryResponse[0];
            subCategory.Name = name;

            // Return Results
            return subCategory;
        }


        /// <summary>
        /// Delete a subcategory
        /// </summary>
        /// <param name="subCategoryId">The id for a specific subcategory</param>
        /// <returns></returns>
        public async Task<bool> DeleteSubCategory(long subCategoryId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("SubCategoryID", subCategoryId);

            _ = await  _core.QueryWebsite<Data.SubCategory>("smugmug.subcategories.delete", queryParams, false);

            // Return True if no error is thrown
            return true;
        }

        /// <summary>
        /// Rename a SubCategory
        /// </summary>
        /// <param name="subCategoryId">The id for a specific subcategory</param>
        /// <param name="newName">The new name to associate with this subcategory</param>
        /// <returns></returns>
        public async Task<bool> RenameSubCategory(long subCategoryId, string newName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("SubCategoryID", subCategoryId);
            queryParams.Add("Name", newName);

            _ = await _core.QueryWebsite<Data.SubCategory>("smugmug.subcategories.rename", queryParams, false);
            
            return true;
        }
    }
}
