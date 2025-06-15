using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Helper class for MethodDetails within OptionsInfo
    public class MethodDetailsInfo
    {
        [JsonPropertyName("Permissions")]
        public List<string> Permissions { get; set; } // [1]
    }
}