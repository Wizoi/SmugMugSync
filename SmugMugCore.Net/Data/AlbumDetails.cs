using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using SmugMug.Net.Data.Domain.Album;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Album Detailed Information
    /// </summary>
    [Serializable(), XmlRoot("Album")]
    public class AlbumDetail
    {
        public AlbumDetail()
        {
            AlbumKey = string.Empty;
        }

        /// <summary>
        /// ID for this Album
        /// </summary>
        [XmlAttribute("id")]
        public int AlbumId;

        /// <summary>
        /// Key for this Album
        /// </summary>
        [XmlAttribute("Key")]
        public string AlbumKey;

        /// <summary>
        /// Allow images from this album to be ranked using PhotoRank
        /// </summary>
        [XmlAttribute("CanRank")]
        public bool CanRank;

        /// <summary>
        /// Allow visitors to leave comments on this album
        /// </summary>
        [XmlAttribute("Comments")]
        public bool CommentsAllowed;

        /// <summary>
        /// Allow EXIF data to be viewed for images from this album.
        /// </summary>
        [XmlAttribute("EXIF")]
        public bool ExifAllowed;

        /// <summary>
        /// Enable mapping features for this album.
        /// </summary>
        [XmlAttribute("Geography")]
        public bool GeographyMappingEnabled;

        /// <summary>
        /// The number of images in this album.
        /// </summary>
        [XmlAttribute("ImageCount")]
        public int ImageCount;

        /// <summary>
        /// The nicename for this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("NiceName")]
        public string? NiceName;

        /// <summary>
        /// Display this album publicly.
        /// </summary>
        [XmlAttribute("Public")]
        public bool PublicDisplay;

        /// <summary>
        /// Display the Share button for this album.
        /// </summary>
        [XmlAttribute("Share")]
        public bool ShareEnabled;

        /// <summary>
        /// The direction used for sorting images within this album. Values: false - Ascending (1-99, A-Z, 1980-2004, etc)true - Descending (99-1, Z-A, 2004-1980, etc)
        /// (owner)
        /// </summary>
        [XmlAttribute("SortDirection")]
        public bool SortDirectionDescending;

        /// <summary>
        /// The method used for sorting images within this album. Values: Position - NoneCaption - By captionFileName - By filenamesDate - By date uploadedDateTime - By date modified (if available)DateTimeOriginal - By date taken (if available)
        /// (owner)
        /// </summary>
        [XmlAttribute("SortMethod")]
        public SortMethod SortMethod;

        /// <summary>
        /// Allow viewing of Large images for this album.
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("Larges")]
        public bool ViewingLargeImagesEnabled;

        /// <summary>
        /// Allow viewing of X2Large images for this album.
        /// </summary>
        [XmlAttribute("X2Larges")]
        public bool ViewingLargeX2ImagesEnabled;

        /// <summary>
        /// Allow viewing of X3Large images for this album.
        /// </summary>
        [XmlAttribute("X3Larges")]
        public bool ViewingLargeX3ImagesEnabled;

        /// <summary>
        /// Allow viewing of XLarge images for this album.
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("XLarges")]
        public bool ViewingLargeXImagesEnabled;

        /// <summary>
        /// Allow viewing of Original images for this album.
        /// </summary>
        [XmlAttribute("Originals")]
        public bool ViewingOriginalImagesEnabled;

        /// <summary>
        /// Images in Album with full information
        /// </summary>
        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public ImageDetail[]? Images;

        /// <summary>
        /// Title for this Album
        /// </summary>
        [XmlAttribute("Title")]
        public string? Title;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public object Copy()
        {
            var newObject = (AlbumDetail) MemberwiseClone();

            return newObject;
        }

    
    }
}


