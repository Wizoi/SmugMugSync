using System.Text.Json.Serialization;
using MetadataExtractor.Formats.Tiff;

namespace SmugMugCore.Net.Data20
{
    public class AlbumDetail
    {
        private string _niceName = string.Empty;

        [JsonPropertyName("NiceName")]
        public string? NiceName {
            get { return _niceName; }
            set
            {
                if (string.IsNullOrEmpty(value) && value.Length > 60)
                {
                    _niceName = value[..60];
                }
                else
                    _niceName = value;            
            }
        } 

        [JsonPropertyName("UrlName")]
        public string? UrlName { get; set; } 

        [JsonPropertyName("Name")] 
        public string? Name { get; set; }

        [JsonPropertyName("AllowDownloads")]
        public bool AllowDownloads { get; set; } 

        [JsonPropertyName("Backprinting")]
        public string? Backprinting { get; set; }

        [JsonPropertyName("BoutiquePackaging")]
        public string? BoutiquePackaging { get; set; } 

        [JsonPropertyName("CanRank")]
        public bool CanRank { get; set; } 

        [JsonPropertyName("Clean")]
        public bool Clean { get; set; } 

        [JsonPropertyName("Comments")]
        public bool Comments { get; set; } 

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("EXIF")]
        public bool EXIF { get; set; } 

        [JsonPropertyName("External")]
        public bool External { get; set; } 

        [JsonPropertyName("FamilyEdit")]
        public bool FamilyEdit { get; set; } 

        [JsonPropertyName("Filenames")]
        public bool Filenames { get; set; } 

        [JsonPropertyName("FriendEdit")]
        public bool FriendEdit { get; set; } 

        [JsonPropertyName("Geography")]
        public bool Geography { get; set; } 

        [JsonPropertyName("Header")]
        public string? Header { get; set; } 

        [JsonPropertyName("HideOwner")]
        public bool HideOwner { get; set; } 

        [JsonPropertyName("InterceptShipping")]
        public string? InterceptShipping { get; set; } 

        [JsonPropertyName("Keywords")]
        public string? Keywords { get; set; } 

        [JsonPropertyName("LargestSize")]
        public string? LargestSize { get; set; } 

        [JsonPropertyName("PackagingBranding")]
        public bool PackagingBranding { get; set; } 

        [JsonPropertyName("Password")]
        public string? Password { get; set; } 

        [JsonPropertyName("PasswordHint")]
        public string? PasswordHint { get; set; } 

        [JsonPropertyName("Printable")]
        public bool Printable { get; set; } 

        [JsonPropertyName("Privacy")]
        public string? Privacy { get; set; } 

        [JsonPropertyName("ProofDays")]
        public int ProofDays { get; set; } 

        [JsonPropertyName("ProofDigital")]
        public bool ProofDigital { get; set; } 

        [JsonPropertyName("Protected")]
        public bool Protected { get; set; } 

        [JsonPropertyName("Share")]
        public bool Share { get; set; } 

        [JsonPropertyName("Slideshow")]
        public bool Slideshow { get; set; } 

        [JsonPropertyName("SmugSearchable")]
        public string? SmugSearchable { get; set; } 

        [JsonPropertyName("SortDirection")]
        public string? SortDirection { get; set; } 

        [JsonPropertyName("SortMethod")]
        public string? SortMethod { get; set; } 

        [JsonPropertyName("SquareThumbs")]
        public bool SquareThumbs { get; set; } 

        [JsonPropertyName("Watermark")]
        public bool Watermark { get; set; } 

        [JsonPropertyName("SecurityType")]
        public string? SecurityType { get; set; } 

        [JsonPropertyName("MaxPhotoDownloadSize")]
        public string? MaxPhotoDownloadSize { get; set; }

        [JsonPropertyName("HighlightAlbumImageUri")]
        public string? HighlightAlbumImageUri { get; set; } 

        [JsonPropertyName("AlbumKey")]
        public string? AlbumKey { get; set; } 

        [JsonPropertyName("CanBuy")]
        public bool CanBuy { get; set; } 

        [JsonPropertyName("CanFavorite")]
        public bool CanFavorite { get; set; } 

        [JsonPropertyName("Date")]
        public string? Date { get; set; } 

        [JsonPropertyName("LastUpdated")]
        public string? LastUpdated { get; set; } 

        [JsonPropertyName("ImagesLastUpdated")]
        public string? ImagesLastUpdated { get; set; } 

        [JsonPropertyName("NodeID")]
        public string? NodeID { get; set; } 

        [JsonPropertyName("OriginalSizes")]
        public long OriginalSizes { get; set; } 

        [JsonPropertyName("TotalSizes")]
        public long TotalSizes { get; set; } 

        [JsonPropertyName("ImageCount")]
        public int ImageCount { get; set; } 

        [JsonPropertyName("UrlPath")]
        public string? UrlPath { get; set; }

        [JsonPropertyName("CanShare")]
        public bool CanShare { get; set; } 

        [JsonPropertyName("HasDownloadPassword")]
        public bool HasDownloadPassword { get; set; } 

        [JsonPropertyName("Packages")]
        public bool Packages { get; set; } 

        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("WebUri")]
        public string? WebUri { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; } 

        [JsonPropertyName("Uris")]
        public AlbumUris? Uris { get; set; }

        [JsonPropertyName("ResponseLevel")]
        public string? ResponseLevel { get; set; }
    }

    public class AlbumUris
    {
        [JsonPropertyName("AlbumShareUris")] 
        public UriLink? AlbumShareUris { get; set; }

        [JsonPropertyName("Node")] 
        public UriLink? Node { get; set; }

        [JsonPropertyName("NodeCoverImage")] 
        public UriLink? NodeCoverImage { get; set; }

        [JsonPropertyName("User")] 
        public UriLink? User { get; set; }

        [JsonPropertyName("Folder")] 
        public UriLink? Folder { get; set; }

        [JsonPropertyName("ParentFolders")] 
        public UriLink? ParentFolders { get; set; }

        [JsonPropertyName("HighlightImage")] 
        public UriLink? HighlightImage { get; set; }

        [JsonPropertyName("AddSamplePhotos")] 
        public UriLink? AddSamplePhotos { get; set; }

        [JsonPropertyName("AlbumHighlightImage")] 
        public UriLink? AlbumHighlightImage { get; set; }

        [JsonPropertyName("AlbumImages")] 
        public UriLink? AlbumImages { get; set; }

        [JsonPropertyName("AlbumPopularMedia")] 
        public UriLink? AlbumPopularMedia { get; set; }

        [JsonPropertyName("AlbumPackages")] 
        public UriLink? AlbumPackages { get; set; }

        [JsonPropertyName("AlbumGeoMedia")] 
        public UriLink? AlbumGeoMedia { get; set; }

        [JsonPropertyName("AlbumComments")] 
        public UriLink? AlbumComments { get; set; }

        [JsonPropertyName("MoveAlbumImages")] 
        public UriLink? MoveAlbumImages { get; set; }

        [JsonPropertyName("CollectImages")] 
        public UriLink? CollectImages { get; set; }

        [JsonPropertyName("ApplyAlbumTemplate")] 
        public UriLink? ApplyAlbumTemplate { get; set; }

        [JsonPropertyName("DeleteAlbumImages")] 
        public UriLink? DeleteAlbumImages { get; set; }

        [JsonPropertyName("UploadFromExternalResource")] 
        public UriLink? UploadFromExternalResource { get; set; }

        [JsonPropertyName("UploadFromUri")] 
        public UriLink? UploadFromUri { get; set; }

        [JsonPropertyName("AlbumGrants")] 
        public UriLink? AlbumGrants { get; set; }

        [JsonPropertyName("AlbumDownload")] 
        public UriLink? AlbumDownload { get; set; }

        [JsonPropertyName("AlbumPrices")] 
        public UriLink? AlbumPrices { get; set; }

        [JsonPropertyName("AlbumPricelistExclusions")] 
        public UriLink? AlbumPricelistExclusions { get; set; }
    }

    public class UriLink
    {
        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("Locator")]
        public string? Locator { get; set; } 

        [JsonPropertyName("LocatorType")]
        public string? LocatorType { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; } 

        [JsonPropertyName("EndpointType")]
        public string? EndpointType { get; set; } 
    }
}