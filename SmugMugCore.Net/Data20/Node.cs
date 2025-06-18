using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
    public class Node
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Privacy")]
        public string? Privacy { get; set; } 

        [JsonPropertyName("Type")]
        public string? Type { get; set; } 

        [JsonPropertyName("UrlName")]
        public string? UrlName { get; set; } 

        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("WebUri")]
        public string? WebUri { get; set; } 

        [JsonPropertyName("DateAdded")]
        public DateTime DateAdded { get; set; } 

        [JsonPropertyName("DateModified")]
        public DateTime DateModified { get; set; } 

        [JsonPropertyName("Uris")]
        public NodeUris? NodeUris { get; set; } 
    }

    public class NodeUris
    {
        [JsonPropertyName("ParentNode")]
        public UriMetadata? ParentNode { get; set; } 

        [JsonPropertyName("ChildNodes")]
        public UriMetadata? ChildNodes { get; set; } 

        [JsonPropertyName("User")]
        public UriMetadata? User { get; set; } 

        [JsonPropertyName("HighlightImage")]
        public UriMetadata? HighlightImage { get; set; } 

        [JsonPropertyName("Album")]
        public UriMetadata? Album { get; set; } 
    }
}