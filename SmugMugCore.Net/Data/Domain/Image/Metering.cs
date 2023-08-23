using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the metering mode set when the image was shot
    public enum Metering
    {
        [XmlEnum(Name = "0")]
        Unknown = 0,
        [XmlEnum(Name = "1")]
        Average = 1,
        [XmlEnum(Name = "2")]
        CenterWeightedAverage = 2,
        [XmlEnum(Name = "3")]
        Spot = 3,
        [XmlEnum(Name = "4")]
        MultiSpot = 4,
        [XmlEnum(Name = "5")]
        Pattern = 5,
        [XmlEnum(Name = "6")]
        Partial = 6,
    };
}
