using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Category")]
    public class UserTreeCategory
    {
        [XmlAttribute("id")]
        public int CategoryId;

        [XmlAttribute("Name")]
        public string? Name;

        [XmlArray("Albums")]
        [XmlArrayItem("Album")]
        public AlbumDetail[]? Albums;

        [XmlArray("SubCategories")]
        [XmlArrayItem("SubCategory")]
        public UserTreeSubCategory[]? SubCategories;

    }
}
