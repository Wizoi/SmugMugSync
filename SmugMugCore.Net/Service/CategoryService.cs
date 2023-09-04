using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [ObsoleteAttribute("smugmug.categories.* no longer is working with v1.3.0 Smugmug API.")]
    public class CategoryService
    {
        private readonly Core.SmugMugCore _core;

        public CategoryService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of categories
        /// </summary>
        /// <returns></returns>
        public async Task<Data.Category[]> GetCategoryList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = await _core.QueryWebsite<Data.Category>("smugmug.categories.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Create a category
        /// </summary>
        /// <param name="name">The name for the category</param>
        /// <param name="niceName">The nicename for the category</param>
        /// <returns></returns>
        public async Task<Data.Category> CreateCategory(string name, string niceName = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("Name", name);
            queryParams.Add("NiceName", niceName, "");

            var queryResponse = await _core.QueryWebsite<Data.Category>("smugmug.categories.create", queryParams, false);
            var category = queryResponse[0];
            
            category.Name = name;
            category.NiceName = niceName;
            category.Type = "<<UNINITIALIZED>>";

            // Return Results
            return category;
        }


        /// <summary>
        /// Retrieve a list of categories
        /// </summary>
        /// <param name="categoryId">The id for a specific category</param>
        /// <returns></returns>
        public async Task<bool> DeleteCategory(long categoryId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CategoryID", categoryId);

            _ = await _core.QueryWebsite<Data.Category>("smugmug.categories.delete", queryParams, false);

            // Return True if no error is thrown
            return true;
        }

        /// <summary>
        /// Rename a Catetory
        /// </summary>
        /// <param name="categoryId">The id for a specific category</param>
        /// <param name="newName">The new name to associate with this category</param>
        /// <returns></returns>
        public async Task<bool> RenameCategory(long categoryId, string newName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("CategoryID", categoryId);
            queryParams.Add("Name", newName);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.categories.rename", queryParams, false);
            return true;
        }

    }
}
