using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SmugMug.Net.Data20
{

    // Represents the structure of an Album object
    // This class includes important fields documented for Albums and also some fields
    // that appear when viewing an Album through the Node endpoint or in search results.
    public class AlbumDetail
    {
        // Fields directly on the Album object from the sample [2, 14, 15] and documentation [12]
        [JsonPropertyName("NiceName")]
        public string NiceName { get; set; } // User-configurable component of the WebUri [12]

        [JsonPropertyName("UrlName")]
        public string UrlName { get; set; } // User-configurable component of the WebUri [12]

        [JsonPropertyName("Title")] // Human-readable title [12, 14] - sample uses Title and Name
        public string Title { get; set; }

        [JsonPropertyName("Name")] // Human-readable title [12, 14] - sample uses Title and Name
        public string Name { get; set; }

        [JsonPropertyName("AllowDownloads")]
        public bool AllowDownloads { get; set; } // [14]

        [JsonPropertyName("Backprinting")]
        public string Backprinting { get; set; } // [14]

        [JsonPropertyName("BoutiquePackaging")]
        public string BoutiquePackaging { get; set; } // [14]

        [JsonPropertyName("CanRank")]
        public bool CanRank { get; set; } // [14]

        [JsonPropertyName("Clean")]
        public bool Clean { get; set; } // [14]

        [JsonPropertyName("Comments")]
        public bool Comments { get; set; } // [14]

        [JsonPropertyName("Description")]
        public string Description { get; set; } // Human-readable description [12, 14]

        [JsonPropertyName("EXIF")]
        public bool EXIF { get; set; } // [14]

        [JsonPropertyName("External")]
        public bool External { get; set; } // Is "allow external embedding" enabled? [12, 14]

        [JsonPropertyName("FamilyEdit")]
        public bool FamilyEdit { get; set; } // [14]

        [JsonPropertyName("Filenames")]
        public bool Filenames { get; set; } // [14]

        [JsonPropertyName("FriendEdit")]
        public bool FriendEdit { get; set; } // [14]

        [JsonPropertyName("Geography")]
        public bool Geography { get; set; } // [14]

        [JsonPropertyName("Header")]
        public string Header { get; set; } // [14]

        [JsonPropertyName("HideOwner")]
        public bool HideOwner { get; set; } // [14]

        [JsonPropertyName("InterceptShipping")]
        public string InterceptShipping { get; set; } // [14]

        [JsonPropertyName("Keywords")]
        public string Keywords { get; set; } // [14]

        [JsonPropertyName("LargestSize")]
        public string LargestSize { get; set; } // [14]

        [JsonPropertyName("PackagingBranding")]
        public bool PackagingBranding { get; set; } // [14]

        [JsonPropertyName("Password")]
        public string Password { get; set; } // [14]

        [JsonPropertyName("PasswordHint")]
        public string PasswordHint { get; set; } // Hint for the viewing password [12, 14]

        [JsonPropertyName("Printable")]
        public bool Printable { get; set; } // [15]

        [JsonPropertyName("Privacy")]
        public string Privacy { get; set; } // Private, Unlisted, or Public [12, 15]

        [JsonPropertyName("ProofDays")]
        public int ProofDays { get; set; } // [15]

        [JsonPropertyName("ProofDigital")]
        public bool ProofDigital { get; set; } // [15]

        [JsonPropertyName("Protected")]
        public bool Protected { get; set; } // [15]

        [JsonPropertyName("Share")]
        public bool Share { get; set; } // [15]

        [JsonPropertyName("Slideshow")]
        public bool Slideshow { get; set; } // [15]

        [JsonPropertyName("SmugSearchable")]
        public string SmugSearchable { get; set; } // Allow in SmugMug search [12, 15]

        [JsonPropertyName("SortDirection")]
        public string SortDirection { get; set; } // [15]

        [JsonPropertyName("SortMethod")]
        public string SortMethod { get; set; } // [15]

        [JsonPropertyName("SquareThumbs")]
        public bool SquareThumbs { get; set; } // [15]

        [JsonPropertyName("Watermark")]
        public bool Watermark { get; set; } // [15]

        [JsonPropertyName("SecurityType")]
        public string SecurityType { get; set; } // [15]

        [JsonPropertyName("MaxPhotoDownloadSize")]
        public string MaxPhotoDownloadSize { get; set; } // [15]

        [JsonPropertyName("HighlightAlbumImageUri")]
        public string HighlightAlbumImageUri { get; set; } // [15]

        [JsonPropertyName("AlbumKey")]
        public string AlbumKey { get; set; } // [15]

        [JsonPropertyName("CanBuy")]
        public bool CanBuy { get; set; } // [15]

        [JsonPropertyName("CanFavorite")]
        public bool CanFavorite { get; set; } // [15]

        // Date fields - using string initially as they are in ISO 8601 format,
        // you can parse them to DateTimeOffset if needed.
        [JsonPropertyName("Date")]
        public string Date { get; set; } // Time created (never changes) [12, 15]

        [JsonPropertyName("LastUpdated")]
        public string LastUpdated { get; set; } // Last change to contents (not ordering) [12, 15]

        [JsonPropertyName("ImagesLastUpdated")]
        public string ImagesLastUpdated { get; set; } // Last change to contents or ordering [12, 15]

        [JsonPropertyName("NodeID")]
        public string NodeID { get; set; } // Appears in sample [2]

        [JsonPropertyName("OriginalSizes")]
        public long OriginalSizes { get; set; } // Appears in sample [2]

        [JsonPropertyName("TotalSizes")]
        public long TotalSizes { get; set; } // Appears in sample [2]

        [JsonPropertyName("ImageCount")]
        public int ImageCount { get; set; } // [2]

        [JsonPropertyName("UrlPath")]
        public string UrlPath { get; set; } // Appears in sample [2]

        [JsonPropertyName("CanShare")]
        public bool CanShare { get; set; } // Appears in sample [2]

        [JsonPropertyName("HasDownloadPassword")]
        public bool HasDownloadPassword { get; set; } // Appears in sample [2]

        [JsonPropertyName("Packages")]
        public bool Packages { get; set; } // Appears in sample [2]

        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The API URI for this album [2]

        [JsonPropertyName("WebUri")]
        public string WebUri { get; set; } // The web URL for this album [2, 12]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // [2]

        // The nested "Uris" object [2]
        [JsonPropertyName("Uris")]
        public AlbumUris Uris { get; set; }

        // ResponseLevel is often present, indicating the verbosity [14]
        [JsonPropertyName("ResponseLevel")]
        public string ResponseLevel { get; set; }
    }

    // Represents the "Uris" object within an Album [2]
    // This contains links to related resources as UriLink objects.
    public class AlbumUris
    {
        [JsonPropertyName("AlbumShareUris")] // [3]
        public UriLink AlbumShareUris { get; set; }

        [JsonPropertyName("Node")] // The Node endpoint view of this album [3, 13]
        public UriLink Node { get; set; }

        [JsonPropertyName("NodeCoverImage")] // [3]
        public UriLink NodeCoverImage { get; set; }

        [JsonPropertyName("User")] // The user who owns this album [4, 13]
        public UriLink User { get; set; }

        [JsonPropertyName("Folder")] // The parent folder of this album [4]
        public UriLink Folder { get; set; }

        [JsonPropertyName("ParentFolders")] // [4]
        public UriLink ParentFolders { get; set; }

        [JsonPropertyName("HighlightImage")] // Image representing the album [5, 13]
        public UriLink HighlightImage { get; set; }

        [JsonPropertyName("AddSamplePhotos")] // [5]
        public UriLink AddSamplePhotos { get; set; }

        [JsonPropertyName("AlbumHighlightImage")] // [6]
        public UriLink AlbumHighlightImage { get; set; }

        [JsonPropertyName("AlbumImages")] // The images in this album as AlbumImage relationships [6, 13]
        public UriLink AlbumImages { get; set; }

        [JsonPropertyName("AlbumPopularMedia")] // [6]
        public UriLink AlbumPopularMedia { get; set; }

        [JsonPropertyName("AlbumPackages")] // Album packages [7]
        public UriLink AlbumPackages { get; set; }

        [JsonPropertyName("AlbumGeoMedia")] // Geotagged images from album [7]
        public UriLink AlbumGeoMedia { get; set; }

        [JsonPropertyName("AlbumComments")] // Comments on album [7]
        public UriLink AlbumComments { get; set; }

        [JsonPropertyName("MoveAlbumImages")] // [8]
        public UriLink MoveAlbumImages { get; set; }

        [JsonPropertyName("CollectImages")] // [8]
        public UriLink CollectImages { get; set; }

        [JsonPropertyName("ApplyAlbumTemplate")] // Apply an album template [8]
        public UriLink ApplyAlbumTemplate { get; set; }

        [JsonPropertyName("DeleteAlbumImages")] // [9]
        public UriLink DeleteAlbumImages { get; set; }

        [JsonPropertyName("UploadFromExternalResource")] // [9]
        public UriLink UploadFromExternalResource { get; set; }

        [JsonPropertyName("UploadFromUri")] // [9]
        public UriLink UploadFromUri { get; set; }

        [JsonPropertyName("AlbumGrants")] // Grants for the Album [10]
        public UriLink AlbumGrants { get; set; }

        [JsonPropertyName("AlbumDownload")] // Download album [10]
        public UriLink AlbumDownload { get; set; }

        [JsonPropertyName("AlbumPrices")] // Purchasable Skus [10]
        public UriLink AlbumPrices { get; set; }

        [JsonPropertyName("AlbumPricelistExclusions")] // Pricelist information [11]
        public UriLink AlbumPricelistExclusions { get; set; }

        // Note: If the request included _shorturis=, this class would instead
        // have string properties like: public string Node { get; set; }
    }

    // A helper class to represent the structure of a URI link when _shorturis is NOT used [3]
    public class UriLink
    {
        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The actual API URI [3]

        [JsonPropertyName("Locator")]
        public string Locator { get; set; } // [3]

        [JsonPropertyName("LocatorType")]
        public string LocatorType { get; set; } // [3]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // [3]

        [JsonPropertyName("EndpointType")]
        public string EndpointType { get; set; } // [3]
    }
}