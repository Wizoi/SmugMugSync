using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Category
    /// </summary>
    [Serializable()]
    public class Category
    {
        /// <summary>
        /// ID for a specific Category
        /// </summary>
        [XmlAttribute("id")]
        public long CategoryId;

        /// <summary>
        /// Name for a specific category
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name;

        /// <summary>
        /// NiceName for a specific category
        /// </summary>
        [XmlAttribute("NiceName")]
        public string? NiceName;

        /// <summary>
        /// Type of Category (Smugmug or User)
        /// </summary>
        [XmlAttribute("Type")]
        public string? Type;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Category Copy()
        {
            var newObject = (Category)MemberwiseClone();
            return newObject;
        }
    }
}
