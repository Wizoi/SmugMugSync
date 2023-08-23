using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    [ObsoleteAttribute("smugmug.albumtemplates.* no longer is working with v1.3.0 Smugmug API.")]
    public class AlbumTemplateService
    {
        private readonly Core.SmugMugCore _core;

        public AlbumTemplateService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Retrieve a list of album templates
        /// </summary>
        /// <returns></returns>
        public Data.AlbumTemplate[] GetAlbumTemplateList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = _core.QueryWebsite<Data.AlbumTemplate>("smugmug.albumtemplates.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Create an album template
        /// </summary>
        /// <returns></returns>
        public Data.AlbumTemplate CreateAlbumTemplate(Data.AlbumTemplate albumTemplate)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            AlbumTemplateService.AddAlbumTemplateParameters(queryParams, albumTemplate);

            var queryResponse = _core.QueryWebsite<Data.AlbumTemplate>("smugmug.albumtemplates.create", queryParams, false);
            var albumTemplateResponse = queryResponse[0];
            var outAlbumTemplate = albumTemplate.Copy();
            outAlbumTemplate.AlbumTemplateId = albumTemplateResponse.AlbumTemplateId;

            // Return Results
            return outAlbumTemplate;
        }


        /// <summary>
        /// Retrieve a list of album templates
        /// </summary>
        /// <returns></returns>
        public bool DeleteAlbumTemplate(int albumTemplateId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumTemplateID", albumTemplateId);

            _core.QueryWebsite<Data.AlbumTemplate>("smugmug.albumtemplates.delete", queryParams, false);

            // Return True if no error is thrown
            return true;
        }

        /// <summary>
        /// Change the settings of an album template
        /// </summary>
        /// <param name="album">Album template object to change settings on</param>
        /// <returns></returns>
        public bool UpdateAlbumTemplate(Data.AlbumTemplate albumTemplate)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumTemplateID", albumTemplate.AlbumTemplateId);
            AlbumTemplateService.AddAlbumTemplateParameters(queryParams, albumTemplate);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.albumtemplates.changeSettings", queryParams, false);
            return true;
        }

        /// <summary>
        /// Add the parameters in an album template to a query string for create/update scenarios
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="album"></param>
        private static void AddAlbumTemplateParameters(Core.QueryParameterList queryParams, Data.AlbumTemplate albumTemplate)
        {
            if (albumTemplate.Template != null)
                queryParams.Add("TemplateID", albumTemplate.Template.TemplateId);
            else
                queryParams.Add("TemplateID", "");
            if (albumTemplate.AlbumTemplateName != null)
                queryParams.Add("AlbumTemplateName", albumTemplate.AlbumTemplateName);
            if (albumTemplate.BackprintingForPrints != null)
                queryParams.Add("Backprinting", albumTemplate.BackprintingForPrints);
            queryParams.Add("CanRank", albumTemplate.CanRank);
            queryParams.Add("Clean", albumTemplate.CleanDisplay);
            queryParams.Add("ColorCorrection", albumTemplate.ColorCorrection);
            queryParams.Add("Comments", albumTemplate.CommentsAllowed);
            if (albumTemplate.Community != null)
                queryParams.Add("CommunityID", albumTemplate.Community.CommunityId);
            else
                queryParams.Add("CommunityID", "");
            queryParams.Add("EXIF", albumTemplate.ExifAllowed);
            queryParams.Add("External", albumTemplate.ExternalLinkAllowed);
            queryParams.Add("FamilyEdit", albumTemplate.FamilyEditAllowed);
            queryParams.Add("Filenames", albumTemplate.FilenameDisplayWhenNoCaptions);
            queryParams.Add("FriendEdit", albumTemplate.FriendEditAllowed);
            queryParams.Add("Geography", albumTemplate.GeographyMappingEnabled);
            queryParams.Add("Header", albumTemplate.HeaderDefaultIsSmugMug);
            queryParams.Add("HideOwner", albumTemplate.HideOwner);
            queryParams.Add("InterceptShipping", albumTemplate.InterceptShippingEnabled);
            queryParams.Add("Larges", albumTemplate.ViewingLargeImagesEnabled);
            queryParams.Add("Originals", albumTemplate.ViewingOriginalImagesEnabled);
            queryParams.Add("PackagingBranding", albumTemplate.PackageBrandedOrdersEnabled);
            if (albumTemplate.Password != null)
                queryParams.Add("Password", albumTemplate.Password);
            if (albumTemplate.PasswordHint != null)
                queryParams.Add("PasswordHint", albumTemplate.PasswordHint);
            queryParams.Add("Position", albumTemplate.Position);
            queryParams.Add("Printable", albumTemplate.PurchaseEnabled);
            if (albumTemplate.PrintmarkApplied != null)
                queryParams.Add("PrintMarkID", albumTemplate.PrintmarkApplied.PrintmarkId);
            else
                queryParams.Add("PrintMarkID", "");
            queryParams.Add("ProofDays", albumTemplate.ProofHoldDays);
            queryParams.Add("Protected", albumTemplate.ProtectionEnabled);
            queryParams.Add("Public", albumTemplate.PublicDisplay);
            queryParams.Add("Share", albumTemplate.ShareEnabled);
            queryParams.Add("SmugSearchable", albumTemplate.SmugSearchEnabled);
            queryParams.Add("SortDirection", albumTemplate.SortDirectionDescending);
            if (albumTemplate.SortMethod != null)
                queryParams.Add("SortMethod", albumTemplate.SortMethod);
            queryParams.Add("SquareThumbs", albumTemplate.SquareThumbnailCropEnabled);
            if (albumTemplate.Template != null)
                queryParams.Add("TemplateID", albumTemplate.Template.TemplateId);
            else
                queryParams.Add("TemplateID", "");
            queryParams.Add("UnsharpAmount", albumTemplate.UnsharpAmount);
            queryParams.Add("UnsharpRadius", albumTemplate.UnsharpRadius);
            queryParams.Add("UnsharpSigma", albumTemplate.UnsharpSigma);
            queryParams.Add("UnsharpThreshold", albumTemplate.UnsharpThreshold);
            if (albumTemplate.Watermark != null)
                queryParams.Add("WatermarkID", albumTemplate.Watermark.WatermarkId);
            else
                queryParams.Add("WatermarkID", "");
            queryParams.Add("WorldSearchable", albumTemplate.WorldSearchableAllowed);
            queryParams.Add("X2Larges", albumTemplate.ViewingLargeX2ImagesEnabled);
            queryParams.Add("X2Larges", albumTemplate.ViewingLargeX3ImagesEnabled);
        }
   }
}
