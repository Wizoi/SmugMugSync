using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class AlbumImageUpdateRequest
    {
        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("Caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("Keywords")]
        public string? Keywords { get; set; }
    }
}