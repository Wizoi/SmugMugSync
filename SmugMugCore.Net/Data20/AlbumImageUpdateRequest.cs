using System.Text.Json.Serialization; 

namespace SmugMug.Net.Data20
{
    public class AlbumImageUpdateRequest
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Caption")]
        public string Caption { get; set; }

        // Keywords are semicolon-separated [3]
        [JsonPropertyName("Keywords")]
        public string Keywords { get; set; }
    }
}