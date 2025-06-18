using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain.Image
{
    // Indicates the kind of light source
    public enum LightSource
    {
        [XmlEnum(Name = "0")]
        Unknown = 0,
        [XmlEnum(Name = "1")]
        Daylight = 1,
        [XmlEnum(Name = "2")]
        Fluorescent = 2,
        [XmlEnum(Name = "3")]
        Tungsten = 3,
        [XmlEnum(Name = "4")]
        Flash = 4,
        [XmlEnum(Name = "9")]
        FineWeather = 9,
        [XmlEnum(Name = "10")]
        CloudyWeather = 10,
        [XmlEnum(Name = "11")]
        Shade = 11,
        [XmlEnum(Name = "12")]
        DaylightFluorescent = 12,
        [XmlEnum(Name = "13")]
        DayWhiteFluorescent = 13,
        [XmlEnum(Name = "14")]
        CoolWhiteFluorescent = 14,
        [XmlEnum(Name = "15")]
        WhiteFluorescent = 15,
        [XmlEnum(Name = "17")]
        StandardLightA = 17,
        [XmlEnum(Name = "18")]
        StandardLightB = 18,
        [XmlEnum(Name = "19")]
        StandardLightC = 19,
        [XmlEnum(Name = "20")]
        D55 = 20,
        [XmlEnum(Name = "21")]
        D65 = 21,
        [XmlEnum(Name = "22")]
        D75 = 22,
        [XmlEnum(Name = "23")]
        D50 = 23,
        [XmlEnum(Name = "24")]
        ISOStudioTungsten = 24,
        [XmlEnum(Name = "255")]
        OtherLightSource = 255,
    };
}
