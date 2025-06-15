using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Represents the "Pages" object used for pagination [1, 2, 4]
    public class PaginationInfo
    {
        [JsonPropertyName("Total")]
        public int Total { get; set; } // The complete number of items available in the total result set [1, 4]

        [JsonPropertyName("Start")]
        public int Start { get; set; } // The 1-based index of the first item in the current page [1, 4]

        [JsonPropertyName("Count")]
        public int Count { get; set; } // The number of items actually returned in this response [1, 4]

        [JsonPropertyName("RequestedCount")]
        public int RequestedCount { get; set; } // The number of items requested via the 'count' parameter [1, 4]

        [JsonPropertyName("FirstPage")]
        public string FirstPage { get; set; } // URI for the first page [1, 4]

        [JsonPropertyName("PrevPage")]
        public string PrevPage { get; set; } // URI for the previous page (may be null/missing if on first page) [1, 4]

        [JsonPropertyName("NextPage")]
        public string NextPage { get; set; } // URI for the next page (may be null/missing if on last page) [1, 2, 4]

        [JsonPropertyName("LastPage")]
        public string LastPage { get; set; } // URI for the last page [1, 2, 4]

        // The Misaligned field is mentioned [4] but not in the sample JSON [1, 2].
        // If you expect it, you might add:
        // [JsonPropertyName("Misaligned")]
        // public bool Misaligned { get; set; }
    }
}