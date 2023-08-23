using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Coupon
{
    /// <summary>
    /// The type of the coupon
    /// </summary>
    public enum CouponType
    {
        [XmlEnum(Name = "Amount")]
        Amount,

        [XmlEnum(Name = "Cost")]
        Cost,
        
        [XmlEnum(Name = "Credit")]
        Credit,
        
        [XmlEnum(Name = "Percent")]
        Percent,
        
        [XmlEnum(Name = "Shipping")]
        Shipping
    };
}
