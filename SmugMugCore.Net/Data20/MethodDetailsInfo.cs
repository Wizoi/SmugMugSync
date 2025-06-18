using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class MethodDetailsInfo
    {
        [JsonPropertyName("Permissions")]
        public List<string>? Permissions { get; set; } // [1]
    }
}