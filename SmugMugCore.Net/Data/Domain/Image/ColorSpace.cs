using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Image
{
    //Indicates the colorspace used by the image
    public enum ColorSpace
    {
        [XmlEnum(Name = "1")]
        sRGB = 1,

        [XmlEnum(Name = "39321")]
        Uncalibrated = 39321,
    };
}

