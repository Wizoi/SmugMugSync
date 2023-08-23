using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Image
{
    // The degrees of rotation
    public enum Degrees
    {
        [XmlEnum(Name = "90")]
        NinetyDegrees = 90,

        [XmlEnum(Name = "180")]
        OneEightyDegrees = 180,
        
        [XmlEnum(Name = "270")]
        TwoSeventyDegrees = 270,
    };
}
