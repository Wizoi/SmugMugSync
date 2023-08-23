using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the direction of saturation processing applied by the camera when the image was shot
    public enum Saturation
    {
        [XmlEnum(Name = "0")]
        Normal = 0,
        [XmlEnum(Name = "1")]
        Low = 1,
        [XmlEnum(Name = "2")]
        HighSaturation = 2,
    };
}
