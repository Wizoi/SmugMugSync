using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class Watermark
    {
        /// <summary>
        /// The id for this watermark
        /// </summary>
        [XmlAttribute("id")]
        public int WatermarkId;

        /// <summary>
        /// The name for this watermark
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name;

        /// <summary>
        /// The opacity of this watermark on the target image
        /// </summary>
        [XmlAttribute("Dissolve")]
        public int Dissolve;

        /// <summary>
        /// Images applying this watermark
        /// </summary>
        [XmlElement("Image")]
        public ImageDetail? Image;

        /// <summary>
        /// The location of this watermark on the target image
        /// </summary>
        [XmlAttribute("Pinned")]
        public SmugMug.Net.Data.Domain.Watermark.Pinned PinnedLocation;

        /// <summary>
        /// This watermark is applied to thumbnail image sizes
        /// </summary>
        [XmlAttribute("Thumbs")]
        public string? ThumbnailWatermarkEnabled;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Watermark Copy()
        {
            var newObject = (Watermark)MemberwiseClone();
            if (this.Image != null)
                newObject.Image = (Data.ImageDetail)this.Image.Copy();
            return newObject;
        }
    }
}
