using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
    public class AlbumImageListResponse
    {
        [JsonPropertyName("Response")]
        public AlbumImageListResponseContent? Response { get; set; }

        [JsonPropertyName("Stat")]
        public string? Status { get; set; }

        [JsonPropertyName("Method")]
        public string? Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string? ErrorMessage { get; set; } 
        
        [JsonPropertyName("Request")]
        public object? Request { get; set; }

        [JsonPropertyName("Options")]
        public object? Options { get; set; }

        public class AlbumImageListResponseContent
        {
            [JsonPropertyName("Uri")]
            public string? Uri { get; set; }

            [JsonPropertyName("Locator")]
            public string? Locator { get; set; }

            [JsonPropertyName("LocatorType")]
            public string? LocatorType { get; set; }

            [JsonPropertyName("AlbumImage")] 
            public List<AlbumImageDetail>? AlbumImages { get; set; }

            [JsonPropertyName("Pages")]
            public PaginationInfo? Pages { get; set; } 

            [JsonPropertyName("Timing")]
            public TimingInfo? Timing { get; set; } 
        }
    }
}