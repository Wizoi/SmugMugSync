using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Helper class for ParameterInfo within OptionsInfo
    public class ParameterInfo
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } // [27, 28]

        [JsonPropertyName("Required")]
        public bool Required { get; set; } // [27, 28]

        [JsonPropertyName("ReadOnly")]
        public bool ReadOnly { get; set; } // [27, 28]

        [JsonPropertyName("Default")]
        public object Default { get; set; } // Can be string, null, etc. [27, 28]

        [JsonPropertyName("Description")]
        public string Description { get; set; } // [28]

        [JsonPropertyName("Type")]
        public string Type { get; set; } // e.g., "Varchar", "Select", "Uri" [27-29]

        [JsonPropertyName("MIN_CHARS")]
        public object MIN_CHARS { get; set; } // Can be int or string ("INFINITY") [27, 28]

        [JsonPropertyName("MAX_CHARS")]
        public object MAX_CHARS { get; set; } // Can be int or string ("INFINITY") [27, 28]

        [JsonPropertyName("OPTIONS")]
        public List<string> OPTIONS { get; set; } // For Type "Select" [28, 29]

        [JsonPropertyName("MIN_COUNT")]
        public int? MIN_COUNT { get; set; } // Optional, for collections [28, 29]

        [JsonPropertyName("MAX_COUNT")]
        public int? MAX_COUNT { get; set; } // Optional, for collections [28, 29]

        [JsonPropertyName("Locator")]
        public List<string> Locator { get; set; } // For Type "Uri" [29]

        [JsonPropertyName("Value")]
        public object Value { get; set; } // Current value in the request [28, 29]

        [JsonPropertyName("Label")]
        public string Label { get; set; } // [27]
    }
}