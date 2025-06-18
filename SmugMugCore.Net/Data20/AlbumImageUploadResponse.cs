using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class AlbumImageUploadResponse
    {
        [JsonPropertyName("Image")]
        public AlbumImageResponse? Image { get; set; }

        [JsonPropertyName("stat")]
        public string? Status { get; set; }

        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("message")]
        public string? ErrorMessage { get; set; } 
    }

    public class AlbumImageResponse
    {
        [JsonPropertyName("ImageUri")]
        public string? ImageUri { get; set; } 

        [JsonPropertyName("AlbumImageUri")]
        public string? AlbumImageUri { get; set; } 

        [JsonPropertyName("StatusImageReplaceUri")]
        public string? StatusImageReplaceUri { get; set; } 

        [JsonPropertyName("URL")]
        public string? GalleryURL { get; set; } 
    }
}