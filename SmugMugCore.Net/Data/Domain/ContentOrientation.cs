using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain
{
    ///<summary>
    ///Image orientation viewed in terms of rows and columns.
    ///</summary>
    public enum ContentOrientation : ushort
    {
        /// <summary>Undefined Orientation</summary>
        Unknown = 0,
        ///<summary>The 0th row is at the top of the visual image, and the 0th column is the visual left side.</summary>
        TopLeft = 1,
        ///<summary>The 0th row is at the visual top of the image, and the 0th column is the visual right side.</summary>
        TopRight = 2,
        ///<summary>The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.</summary>
        BottomLeft = 3,
        ///<summary>The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.</summary>
        BottomRight = 4,
        ///<summary>The 0th row is the visual left side of the image, and the 0th column is the visual top.</summary>
        LeftTop = 5,
        ///<summary>The 0th row is the visual right side of the image, and the 0th column is the visual top.</summary>
        RightTop = 6,
        ///<summary>The 0th row is the visual right side of the image, and the 0th column is the visual bottom.</summary>
        RightBottom = 7,
        ///<summary>The 0th row is the visual left side of the image, and the 0th column is the visual bottom.</summary>
        LeftBottom = 8
    }
}
