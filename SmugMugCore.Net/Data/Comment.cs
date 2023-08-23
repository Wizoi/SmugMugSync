using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Comment for an Image or an Album
    /// </summary>
    [Serializable()]
    public class Comment
    {
        public Comment() {
            this.Text = string.Empty;
            this.Type = string.Empty;
        }

        /// <summary>
        /// ID for a specific Comment
        /// </summary>
        [XmlAttribute("id")]
        public int CommentId;

        /// <summary>
        /// Date comment was posted
        /// </summary>
        [XmlAttribute("Date")]
        public string? DatePosted;

        /// <summary>
        /// Rating provided in comment (1-5)
        /// </summary>
        [XmlAttribute("Rating")]
        public int Rating;

        /// <summary>
        /// Comment Text
        /// </summary>
        [XmlAttribute("Text")]
        public string Text;

        /// <summary>
        /// Type of Comment
        /// </summary>
        [XmlAttribute("Type")]
        public string Type;

        /// <summary>
        /// User who posted comment
        /// </summary>
        [XmlElement("User")]
        public Data.User? User;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Comment Copy()
        {
            var newObject = (Comment)MemberwiseClone();
            return newObject;
        }

    }

}
