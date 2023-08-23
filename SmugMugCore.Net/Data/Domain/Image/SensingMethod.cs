using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the image sensor type on the camera or input device
    public enum SensingMethod
    {
        [XmlEnum(Name = "0")]
        Unknown = 0,
        [XmlEnum(Name = "1")]
        NotDefined = 1,
        [XmlEnum(Name = "2")]
        OneChipColorAreaSensor = 2,
        [XmlEnum(Name = "3")]
        TwoChipColorAreaSensor = 3,
        [XmlEnum(Name = "4")]
        ThreeChipColorAreaSensor = 4,
        [XmlEnum(Name = "5")]
        ColorSequentialAreaSensor = 5,
        [XmlEnum(Name = "6")]
        TrilinearSensor = 7,
        [XmlEnum(Name = "7")]
        ColorSequentialLinearSensor = 8,
    }
}
