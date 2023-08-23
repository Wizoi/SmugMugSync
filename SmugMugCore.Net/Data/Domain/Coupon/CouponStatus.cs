using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Coupon
{
    /// <summary>
    /// The status of the coupon
    /// </summary>
    public enum CouponStatus
    {
        [XmlEnum(Name = "Active")]
        Active,

        [XmlEnum(Name = "Consumed")]
        Consumed,

        [XmlEnum(Name = "Disabled")]
        Disabled,
        
        [XmlEnum(Name = "Expired")]
        Expired,
        
        [XmlEnum(Name = "Inactive")]
        Inactive
    };
}

