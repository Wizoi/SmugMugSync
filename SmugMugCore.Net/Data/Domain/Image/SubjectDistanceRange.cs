using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain.Image
{
    // Indicates the distance to the subject
    public enum SubjectDistanceRange
    {
        [XmlEnum(Name = "0")]
        Unknown = 0,
        
        [XmlEnum(Name = "1")]
        Macro = 1,
        
        [XmlEnum(Name = "2")]
        CloseView = 2,
        
        [XmlEnum(Name = "3")]
        DistantView = 3,
    };
}
