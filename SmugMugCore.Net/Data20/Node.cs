using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    /// <summary>
    /// Represents a Node object, which can be a Folder, Album, or Page. [35]
    /// When creating an album via ChildNodes, the 'Type' field will be "Album". [8]
    /// </summary>
    public class Node
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } // Human-readable title [17, 35]

        [JsonPropertyName("Description")]
        public string Description { get; set; } // Human-readable description [17, 35]

        [JsonPropertyName("Privacy")]
        public string Privacy { get; set; } // "Public", "Unlisted", or "Private" [17, 35]

        [JsonPropertyName("Type")]
        public string Type { get; set; } // Indicates the type of node: "Folder", "Album", or "Page" [17, 35]. When creating an album node, this will be "Album".

        [JsonPropertyName("UrlName")]
        public string UrlName { get; set; } // User-configurable part of the WebUri [17, 35]

        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The API URI for this specific node object [17, 35]

        [JsonPropertyName("WebUri")]
        public string WebUri { get; set; } // The public web URL for the node [17, 35]

        [JsonPropertyName("DateAdded")]
        public DateTime DateAdded { get; set; } // Timestamp when this node was created [17, 35]

        [JsonPropertyName("DateModified")]
        public DateTime DateModified { get; set; } // Timestamp of the last modification to this node or its descendants [17, 35]

        [JsonPropertyName("Uris")]
        public NodeUris NodeUris { get; set; } // Collection of related API URIs for the node [17]

        // Other properties include: HideOwner, Keywords, PasswordHint, SecurityType, SmugSearchable,
        // SortDirection, SortMethod, WorldSearchable, EffectivePrivacy, EffectiveSecurityType,
        // FormattedValues, HasChildren, IsRoot, SortIndex, UrlPath, ResponseLevel. [17, 35]
    }

    /// <summary>
    /// Represents the collection of related API URIs for a Node.
    /// </summary>
    public class NodeUris
    {
        [JsonPropertyName("ParentNode")]
        public UriMetadata ParentNode { get; set; } // The parent node URI [17, 36]

        [JsonPropertyName("ChildNodes")]
        public UriMetadata ChildNodes { get; set; } // URI to get child nodes [17, 36]

        [JsonPropertyName("User")]
        public UriMetadata User { get; set; } // The owning user's URI [17, 36]

        [JsonPropertyName("HighlightImage")]
        public UriMetadata HighlightImage { get; set; } // URI for the node's highlight image [17, 36]

        [JsonPropertyName("Album")]
        public UriMetadata Album { get; set; } // Present if this Node is an album, linking to the Album object [36]

        // Other linked URIs: FolderByID, ParentNodes, MoveNodes, NodeGrants
    }
}