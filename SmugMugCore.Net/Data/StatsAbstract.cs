using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace SmugMug.Net.Data
{
    [Serializable()]
    public abstract class StatsAbstract
    {
        /// <summary>
        /// The bytes transferred for this album for a given month
        /// </summary>
        [XmlAttribute("Bytes")]
        public int Bytes;

        /// <summary>
        /// The total hits for this album for a given month
        /// </summary>
        [XmlAttribute("Hits")]
        public int Hits;

        /// <summary>
        /// The number of Large hits for this album for a given month
        /// </summary>
        [XmlAttribute("Large")]
        public int ViewingLargeHits;

        /// <summary>
        /// The number of Medium hits for this album for a given month
        /// </summary>
        [XmlAttribute("Medium")]
        public int ViewingMediumHits;

        /// <summary>
        /// The number of Original hits for this album for a given month
        /// </summary>
        [XmlAttribute("Original")]
        public int ViewingOriginalHits;

        /// <summary>
        /// The number of XLarge hits for this album for a given month
        /// </summary>
        [XmlAttribute("XLarge")]
        public int ViewingXLarge;

        /// <summary>
        /// The number of X2Large hits for this album for a given month
        /// </summary>
        [XmlAttribute("X2Large")]
        public int ViewingX2Large;

        /// <summary>
        /// The number of X3Large hits for this album for a given month
        /// </summary>
        [XmlAttribute("X3Large")]
        public int ViewingX3Large;

        /// <summary>
        /// The number of Small hits for this album for a given month
        /// </summary>
        [XmlAttribute("Small")]
        public int ViewingSmallHits;

        /// <summary>
        /// The number of Video1280 hits for this album for a given month
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("Video1280")]
        public int Video1280Hits;

        /// <summary>
        /// The number of Video1920 hits for this album for a given month
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("Video1920")]
        public int Video1920Hits;

        /// <summary>
        /// The number of Video320 hits for this album for a given month
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("Video320")]
        public int Video320Hits;

        /// <summary>
        /// The number of Video640 hits for this album for a given month
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("Video640")]
        public int Video640Hits;

        /// <summary>
        /// The number of Video960 hits for this album for a given month
        /// (pro accounts only) 
        /// </summary>
        [XmlAttribute("Video960")]
        public int Video960Hits;

        /// <summary>
        /// Images of the Album and their stats, if the user chooses to load these
        /// </summary>
        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public ImageStats[]? ImageStats;
    }
}
