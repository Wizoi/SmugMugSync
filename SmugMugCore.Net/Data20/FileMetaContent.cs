using System.IO.Abstractions;

namespace SmugMugCore.Net.Data20
{
    /// <summary>
    /// Class used for uploading an image
    /// </summary>
    public class FileMetaContent
    {
        /// <summary>
        /// Image Altitude in Meters when taken
        /// </summary>
        public double GeoAltitude;
        
        /// <summary>
        /// Image Latitude when taken
        /// </summary>
        public double GeoLatitude;

        /// <summary>
        /// Image Longitude when taken
        /// </summary>
        public double GeoLongitude;

        /// <summary>
        /// Caption of the Image
        /// </summary>
        public string? Title;

        /// <summary>
        /// Caption of the Image
        /// </summary>
        public string? Caption;

        /// <summary>
        /// Whether this image should be hidden from public viewing
        /// </summary>
        public bool IsHidden;

        /// <summary>
        /// Keywords to associate with image
        /// </summary>
        public string[] Keywords;

        /// <summary>
        /// Filesystem Properties for this Image
        /// </summary>
        public IFileInfo? FileInfo;

        /// <summary>
        /// Computed MD5 Checksum for this image
        /// </summary>
        public string? MD5Checksum;

        /// <summary>
        /// When the photo was taken
        /// </summary>
        public DateTime DateTaken;

        /// <summary>
        /// Comment or Photo Extended Description
        /// </summary>
        public string? Comment;

        /// <summary>
        /// Boolean to return if the content is a video file
        /// </summary>
        public bool IsVideo;

        /// <summary>
        /// If this is a video, return the length of the video
        /// </summary>
        public TimeSpan VideoLength = new ();

        /// <summary>
        /// Orientation of the content
        /// </summary>
        public ImageContentOrientation Orientation;

        public FileMetaContent() {
            this.Keywords = [];
        }
    }

  
}
