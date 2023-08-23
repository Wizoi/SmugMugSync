using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Image")]
    public class ImageDetail
    {
        public ImageDetail()
        {
            this.ImageKey = string.Empty;
            this.Filename = string.Empty;
            this.UrlDefault = string.Empty;
            this.UrlLightboxURL = string.Empty; 
            this.UrlVideo1280URL = string.Empty;
            this.UrlVideo1920URL = string.Empty;
            this.UrlVideo320URL = string.Empty;
            this.UrlVideo640URL = string.Empty;
            this.UrlVideo960URL = string.Empty;
            this.UrlVideoSMILURL = string.Empty;
            this.UrlViewLargeURL = string.Empty;
            this.UrlViewMediumURL = string.Empty;
            this.UrlViewOriginalURL = string.Empty;
            this.UrlViewSmallURL = string.Empty;
            this.UrlViewThumbURL = string.Empty;
            this.UrlViewTinyURL = string.Empty;
            this.UrlViewX2LargeURL = string.Empty;
            this.UrlViewX3LargeURL = string.Empty;
            this.UrlViewXLargeURL = string.Empty;
            }

        /// <summary>
        /// Cleaned filename base for the filename
        /// </summary>
        public string FileNameBase
        {
            get
            {
                return
                    Path.GetFileNameWithoutExtension(
                      System.Web.HttpUtility.HtmlDecode(this.Filename)).
                    ToUpper();
            }
        }

        /// <summary>
        /// The id for this image
        /// </summary>
        [XmlAttribute("id")]
        public long ImageId;

        /// <summary>
        /// The key for this image
        /// </summary>
        [XmlAttribute("Key")]
        public string ImageKey;

        /// <summary>
        /// Album details when images are dynamically put into an album, but are realy sourced from a different album.
        /// (collected & smart only) 
        /// </summary>
        [XmlElement("Album")]
        public AlbumDetail? Album;

        /// <summary>
        /// The altitude at which this image (or video) was taken
        /// </summary>
        [XmlAttribute("Altitude")]
        public int Altitude;

        /// <summary>
        /// The caption for this image (or video)
        /// </summary>
        [XmlAttribute("Title")]
        public string? Title;

        /// <summary>
        /// The caption for this image (or video)
        /// </summary>
        [XmlAttribute("Caption")]
        public string? Caption;

        /// <summary>
        /// The date that this image (or video) was uploaded
        /// (owner)
        /// </summary>
        [XmlAttribute("Date")]
        public string? DateUploaded;

        /// <summary>
        /// The filename of this image (or video)
        /// </summary>
        [XmlAttribute("FileName")]
        public string Filename;

        /// <summary>
        /// Duration integer 
        /// (video, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("Duration")]
        public float DurationSeconds;

        /// <summary>
        /// The original format of this image (or video)
        /// </summary>
        [XmlAttribute("Format")]
        public string? Format;

        /// <summary>
        /// The height of the crop
        /// </summary>
        [XmlAttribute("Height")]
        public int Height;

        /// <summary>
        /// Hide this image (or video)
        /// (owner)
        /// </summary>
        [XmlAttribute("Hidden")]
        public bool Hidden;

        /// <summary>
        /// The keyword string for this image (or video)
        /// </summary>
        [XmlAttribute("Keywords")]
        public string? Keywords;

        /// <summary>
        /// The date that this image (or video) was last updated
        /// </summary>
        [XmlAttribute("LastUpdated")]
        public string? LastUpdatedDate;

        /// <summary>
        /// The latitude at which this image (or video) was taken
        /// </summary>
        [XmlAttribute("Latitude")]
        public float Latitude;

        /// <summary>
        /// The Longitude at which this image (or video) was taken
        /// </summary>
        [XmlAttribute("Longitude")]
        public float Longitude;

        /// <summary>
        /// The MD5 sum for this image (or video)
        /// (owner)
        /// </summary>
        [XmlAttribute("MD5Sum")]
        public string? MD5Sum;

        /// <summary>
        /// The position of this image (or video) within the album
        /// </summary>
        [XmlAttribute("Position")]
        public int PositionInAlbum;

        /// <summary>
        /// The current revision of this image (or video)
        /// </summary>
        [XmlAttribute("Serial")]
        public int Revision;

        /// <summary>
        /// The size of this image (or video) in bytes
        /// </summary>
        [XmlAttribute("Size")]
        public int SizeBytes;

        /// <summary>
        /// The width of the crop
        /// </summary>
        [XmlAttribute("Width")]
        public int Width;

        /// Default Image URL
        /// </summary>
        [XmlAttribute("URL")]
        public string? UrlDefault;

        /// The Lightbox URL for this image
        /// </summary>
        [XmlAttribute("LightboxURL")]
        public string UrlLightboxURL;

        /// <summary>
        /// The url for the Large version of this image
        /// </summary>
        [XmlAttribute("LargeURL")]
        public string UrlViewLargeURL;

        /// <summary>
        /// The url for the Medium version of this image
        /// </summary>
        [XmlAttribute("MediumURL")]
        public string UrlViewMediumURL;

        /// <summary>
        /// The url for the Original version of this image
        /// </summary>
        [XmlAttribute("OriginalURL")]
        public string UrlViewOriginalURL;

        /// <summary>
        /// The url for the Small version of this image
        /// </summary>
        [XmlAttribute("SmallURL")]
        public string UrlViewSmallURL;

        /// <summary>
        /// The url for the Thumb version of this image
        /// </summary>
        [XmlAttribute("ThumbURL")]
        public string UrlViewThumbURL;

        /// <summary>
        /// The url for the Tiny version of this image
        /// </summary>
        [XmlAttribute("TinyURL")]
        public string UrlViewTinyURL;

        /// <summary>
        /// The url for the X2Large version of this image
        /// </summary>
        [XmlAttribute("X2LargeURL")]
        public string UrlViewX2LargeURL;

        /// <summary>
        /// The url for the X3Large version of this image
        /// </summary>
        [XmlAttribute("X3LargeURL")]
        public string UrlViewX3LargeURL;

        /// <summary>
        /// The url for the XLarge version of this image
        /// </summary>
        [XmlAttribute("XLargeURL")]
        public string UrlViewXLargeURL;

        /// <summary>
        /// The url for the 320 version of this video.
        /// </summary>
        [XmlAttribute("Video320URL")]
        public string UrlVideo320URL;

        /// <summary>
        /// The url for the 640 version of this video
        /// </summary>
        [XmlAttribute("Video640URL")]
        public string UrlVideo640URL;

        /// <summary>
        /// The url for the 960 version of this video.
        /// </summary>
        [XmlAttribute("Video960URL")]
        public string UrlVideo960URL;

        /// <summary>
        /// The url for the 1280 version of this video.
        /// </summary>
        [XmlAttribute("Video1280URL")]
        public string UrlVideo1280URL;

        /// <summary>
        /// The url for the 1920 version of this video
        /// </summary>
        [XmlAttribute("Video1920URL")]
        public string UrlVideo1920URL;

        /// <summary>
        /// The url for the W3C SMIL XML Link for the Videos
        /// </summary>
        [XmlAttribute("VideoSMILURL")]
        public string UrlVideoSMILURL;

        /// <summary>
        /// Comments for the Image
        /// </summary>
        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public Comment[]? Comments;

        /// <summary>
        /// Client side property to indicate when this has been removed on remote site
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public object Copy()
        {
            var newObject = MemberwiseClone();
            return newObject;
        }
    }
}
