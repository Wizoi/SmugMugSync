using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
    public class ImageDetailResponse
    {
        [JsonPropertyName("Response")]
        public ImageDetailResponseContent? Response { get; set; }

        [JsonPropertyName("Stat")]
        public string? Status { get; set; }

        [JsonPropertyName("Method")]
        public string? Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string? ErrorMessage { get; set; } 

        [JsonPropertyName("Options")]
        public object? Options { get; set; }

        public class ImageDetailResponseContent
        {
            [JsonPropertyName("Uri")]
            public string? Uri { get; set; }

            [JsonPropertyName("Locator")]
            public string? Locator { get; set; }

            [JsonPropertyName("LocatorType")]
            public string? LocatorType { get; set; }

            [JsonPropertyName("Image")]
            public ImageDetail? Image { get; set; }
        }
    }
}