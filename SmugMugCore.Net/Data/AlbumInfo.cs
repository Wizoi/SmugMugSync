using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace SmugMug.Net.Data
{
    /// <summary>
    /// Album Detailed Information
    /// </summary>
    [Serializable(), XmlRoot("Album")]
    public class AlbumInfo : Album
    {
        /// <summary>
        /// The text to be printed on the back of prints purchased from this album
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("Backprinting")]
        public int BackprintingForPrints;

        /// <summary>
        /// Enable boutique packaging for orders from this album
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("BoutiquePackaging")]
        public string? BoutiquePackagingForOrders;
        
        /// <summary>
        /// Allow images from this album to be ranked using PhotoRank
        /// </summary>
        [XmlAttribute("CanRank")]
        public bool CanRank;

        /// <summary>
        /// Hide the Description and LastUpdated for this album on the homepage and category pages
        /// </summary>
        [XmlAttribute("CleanDisplay")]
        public bool CleanDisplay;

        /// <summary>
        /// The color correction setting for this album
        /// (owner) 
        /// </summary>
        [XmlAttribute("ColorCorrection")]
        public int ColorCorrection;

        /// <summary>
        /// Allow visitors to leave comments on this album
        /// </summary>
        [XmlAttribute("Comments")]
        public bool CommentsAllowed;

        /// <summary>
        /// The description for this album.
        /// </summary>
        [XmlAttribute("Description")]
        public string? Description;
        
        /// <summary>
        /// Allow EXIF data to be viewed for images from this album.
        /// </summary>
        [XmlAttribute("EXIF")]
        public bool ExifAllowed;

        /// <summary>
        /// Allow images from this album to be linked externally outside SmugMug.
        /// </summary>
        [XmlAttribute("External")]
        public bool ExternalLinkAllowed;

        /// <summary>
        /// Allow family to edit the captions and keywords of the images in this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("FamilyEdit")]
        public bool FamilyEditAllowed;

        /// <summary>
        /// Show filename for images uploaded with no caption to this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("Filenames")]
        public bool FilenameDisplayWhenNoCaptions;

        /// <summary>
        /// Allow friends to edit the captions and keywords of the images in this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("FriendEdit")]
        public bool FriendEditAllowed;
        
        /// <summary>
        /// Enable mapping features for this album.
        /// </summary>
        [XmlAttribute("Geography")]
        public bool GeographyMappingEnabled;

        /// <summary>
        /// Default this album to the standard SmugMug appearance. Values: false - Customtrue - SmugMug
        /// (owner, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("Header")]
        public bool HeaderDefaultIsSmugMug;

        /// <summary>
        /// Hide the owner of this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("HideOwner")]
        public bool HideOwner;

        /// <summary>
        /// The number of images in this album.
        /// </summary>
        [XmlAttribute("ImageCount")]
        public int ImageCount;

        /// <summary>
        /// Enable intercept shipping (personal delivery) for orders from this album. Values: 0 - No1 - Yes2 - Inherit
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("InterceptShipping")]
        public int InterceptShippingEnabled;

        /// <summary>
        /// The meta keyword string for this album.
        /// </summary>
        [XmlAttribute("Keywords")]
        public string? Keywords;

        /// <summary>
        /// The date that this album was last updated.
        /// </summary>
        [XmlAttribute("LastUpdated")]
        public string? LastUpdated;

        /// <summary>
        /// The nicename for this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("NiceName")]
        public string? NiceName;

        /// <summary>
        /// Enable packaging branding for orders from this album.
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("PackagingBranding")]
        public bool PackageBrandedOrdersEnabled;

        /// <summary>
        /// The password for this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("Password")]
        public string? Password;

        /// <summary>
        /// The password hint for this album.
        /// </summary>
        [XmlAttribute("PasswordHint")]
        public string? PasswordHint;

        /// <summary>
        /// Indicates whether this album is password protected.
        /// </summary>
        [XmlAttribute("Passworded")]
        public string? PasswordProtected;

        /// <summary>
        /// The position of this album within the site.
        /// </summary>
        [XmlAttribute("Position")]
        public int Position;

        /// <summary>
        /// Allow images from this album to purchased as a print, merchandise or digital download.
        /// </summary>
        [XmlAttribute("Printable")]
        public int PurchaseEnabled;

        /// <summary>
        /// The number of days an order is held for a pro to proof prior to being sent to print.
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("ProofDays")]
        public int ProofHoldDays;

        /// <summary>
        /// Enable right-click protection for this album.
        /// (power & pro accounts only) 
        /// </summary>
        [XmlAttribute("Protected")]
        public bool ProtectionEnabled;

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
        /// Allow SmugMug to index images from this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("SmugSearchable")]
        public bool SmugSearchEnabled;

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
        public string? SortMethod;

        /// <summary>
        /// Enable automatic square cropping of thumbnails for this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("SquareThumbs")]
        public bool SquareThumbnailCropEnabled;

        /// <summary>
        /// The Amount setting used for sharpening display copies of images in this album.
        /// (owner, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("UnsharpAmount")]
        public float UnsharpAmount;

        /// <summary>
        /// The Radius setting used for sharpening display copies of images in this album.
        /// (owner, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("UnsharpRadius")]
        public float UnsharpRadius;

        /// <summary>
        /// The Sigma setting used for sharpening display copies of images in this album.
        /// (owner, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("UnsharpSigma")]
        public float UnsharpSigma;
        
        /// <summary>
        /// The Threshold setting used for sharpening display copies of images in this album.
        /// (owner, power & pro accounts only) 
        /// </summary>
        [XmlAttribute("UnsharpThreshold")]
        public float UnsharpThreshold;

        /// <summary>
        /// Enable automatic watermarking of images for this album.
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlAttribute("Watermarking")]
        public bool WatermarkingEnabled;

        /// <summary>
        /// Allow search engines to index images from this album.
        /// (owner)
        /// </summary>
        [XmlAttribute("WorldSearchable")]
        public bool WorldSearchableAllowed;

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
        /// The community that this album belongs to
        /// (owner) 
        /// </summary>
        [XmlElement("Community")]
        public Community? Community;

        /// <summary>
        /// The highlight image for this album 
        /// </summary>
        [XmlElement("Highlight")]
        public Highlight? Highlight;

        /// <summary>
        /// The printmark applied to images of this album.
        /// (owner, pro accounts only) 
        /// </summary>
        [XmlElement("Printmark")]
        public PrintmarkCore? PrintmarkApplied;

        /// <summary>
        /// The style template applied to this album.
        /// (owner)
        /// </summary>
        [XmlElement("Template")]
        public Template? Template;

        /// <summary>
        /// The theme applied to this album.
        /// (owner)
        /// </summary>
        [XmlElement("Theme")]
        public Theme? Theme;

        /// <summary>
        /// The watermark applied to images of this album.
        /// </summary>
        [XmlElement("Watermark")]
        public Watermark? Watermark;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public override object Copy()
        {
            var newObject = (AlbumInfo) MemberwiseClone();
            if (this.Category != null)
                newObject.Category = this.Category.Copy();
            if (this.SubCategory != null)
                newObject.SubCategory = this.SubCategory.Copy();
            if (this.Community != null)
                newObject.Community = this.Community.Copy();
            if (this.Highlight != null)
                newObject.Highlight = this.Highlight.Copy();
            if (this.PrintmarkApplied != null)
                newObject.PrintmarkApplied = (Data.PrintmarkCore) this.PrintmarkApplied.Copy();
            if (this.Template != null)
                newObject.Template = this.Template.Copy();
            if (this.Theme != null)
                newObject.Theme = this.Theme.Copy();
            if (this.Watermark != null)
                newObject.Watermark = this.Watermark.Copy();

            return newObject;
        }
    }
}
