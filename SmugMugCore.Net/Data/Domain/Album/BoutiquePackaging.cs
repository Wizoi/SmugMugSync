using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Album
{
    public enum BoutiquePackaging
    {
        [XmlEnum(Name = "0")]
        No = 0,
        [XmlEnum(Name = "1")]
        Yes = 1,
        [XmlEnum(Name = "2")]
        Inherit = 2,
    };
}
