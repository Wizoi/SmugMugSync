using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Represents the "Options" object [1]
    // This object details methods, media types, parameters, etc. [26]
    public class OptionsInfo
    {
        [JsonPropertyName("Methods")]
        public List<string> Methods { get; set; } // Supported HTTP methods [1, 26]

        [JsonPropertyName("ParameterDescription")]
        public Dictionary<string, string> ParameterDescription { get; set; } // [1, 27]

        [JsonPropertyName("Parameters")]
        public Dictionary<string, List<ParameterInfo>> Parameters { get; set; } // [27-29]
    }
}