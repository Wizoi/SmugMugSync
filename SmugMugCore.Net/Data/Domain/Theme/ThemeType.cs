using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Theme
{
    public enum ThemeType
    {
        [XmlEnum(Name = "SmugMug")]
        SmugMug,
        
        [XmlEnum(Name = "User")]
        User
    };
}
