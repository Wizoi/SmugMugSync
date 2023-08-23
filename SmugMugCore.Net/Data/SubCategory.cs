using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class SubCategory
    {
        [XmlAttribute("id")]
        public long SubCategoryId;

        [XmlAttribute("Name")]
        public string? Name;

        [XmlAttribute("NiceName")]
        public string? NiceName;

        [XmlElement("Category")]
        public Category? Category;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public SubCategory Copy()
        {
            var newObject = (SubCategory)MemberwiseClone();
            if (this.Category != null)
                newObject.Category = this.Category.Copy();
            return newObject;
        }
    }
}
