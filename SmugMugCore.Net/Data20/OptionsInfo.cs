using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMugCore.Net.Data20
{
    public class OptionsInfo
    {
        [JsonPropertyName("Methods")]
        public List<string>? Methods { get; set; } 

        [JsonPropertyName("ParameterDescription")]
        public Dictionary<string, string>? ParameterDescription { get; set; } 

        [JsonPropertyName("Parameters")]
        public Dictionary<string, List<ParameterInfo>>? Parameters { get; set; } 
    }
}