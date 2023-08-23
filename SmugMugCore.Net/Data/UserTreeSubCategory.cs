using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("SubCategory")]
    public class UserTreeSubCategory
    {
        [XmlAttribute("id")]
        public int SubCategoryId;

        [XmlAttribute("Name")]
        public string? Name;

        [XmlArray("Albums")]
        [XmlArrayItem("Album")]
        public AlbumDetail[]? Albums;
    }
}
