using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Core Album Functionality
    /// </summary>
    [Serializable()]
    public class Album : AlbumCore
    {
        public Album(): base() { 
            this.Title = string.Empty;
        }
        /// <summary>
        /// Title for this Album
        /// </summary>
        [XmlAttribute("Title")]
        public string Title;

        /// <summary>
        /// Category for this Album
        /// </summary>
        [XmlElement("Category")]
        public Category? Category;

        /// <summary>
        /// Subcategory for this Album
        /// </summary>
        [XmlElement("SubCategory")]
        public SubCategory? SubCategory;

    }
}
