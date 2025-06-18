using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain.Token
{
    // The permissions for this OAuth access token
    public enum Permissions
    {
        Read,
        Add,
        Modify,
    };
}
