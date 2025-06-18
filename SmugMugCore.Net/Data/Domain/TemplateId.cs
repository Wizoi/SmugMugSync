using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain
{

    public enum TemplateId
    {
        ViewerChoice = 0, //Default
        SmugMug = 3,
        Traditional = 4,
        AllThumbs = 7,
        Slideshow = 8,
        Journal = 9,
        SmugMugSmall = 10,
        Filmstrip = 11,
    };

}
