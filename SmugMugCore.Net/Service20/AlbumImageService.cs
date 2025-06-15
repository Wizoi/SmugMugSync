using System.Text.Json;
using System.Web;
using RestSharp;
using SmugMug.Net.Data20;

namespace SmugMug.Net.Service20
{
    public class AlbumImageService
    {
        public readonly string API_ALBUM_SEARCH = "/api/v2/album/{0}!images";
        public readonly string API_ALBUM_IMAGE_LOOKUP = "/api/v2/album/{0}/image/{1}";
        public readonly string ROOT_SCOPE = "/api/v2/user/{0}";

        /*
                ImageService.UpdateImage(ImagObject)
                ImageService.Delete(ImageId, AlbumId)

                ImageService.GetAlbumImages(AlbumId, AlbumKey,
                                    fieldList: new string[] { "Filename", "Name", "Title", "Caption", "SizeBytes", "MD5Sum", "Keywords" })
                done ImageService.GetImageInfo(ImageId, ImageKey)
                ImageService.DownloadImage(ImagObject, FullFileName)
        */

        private readonly Core20.SmugMugCore _core;

        public AlbumImageService(Core20.SmugMugCore core)
        {
            _core = core;
        }
        
        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<Data20.AlbumImage> GetImageDetail(string albumKey, string imageKey)
        {
            return await GetImageDetail(string.Format(API_ALBUM_IMAGE_LOOKUP, albumKey, imageKey));
        }

        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<Data20.AlbumImage> GetImageDetail(string albumImageUri)
        {
            var request = new RestRequest(albumImageUri, Method.Get);
            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumImageDetailResponse>();
                return rawResponse.Response.AlbumImage;
            }

            throw new Exception("Error loading album image: " + restResponse.ErrorMessage);
        }

    }
}
