using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace SmugMugCore.Net.Core
{
    /// <summary>
    /// Map a SmugMug Error code to a more descriptive name
    /// </summary>
    public enum ErrorCode
    {
        None = 0,
        InvalidSession = 3,
        MessageInvalidUser = 4,
        SystemError = 5,
        EmptySetNoChildrenFound = 15, // Strict Only
        MessageInvalidData = 16,
        InvalidMethod = 17,
        InvalidApiKey = 18,
        DataNotChanged = 21, // Strict only
        MessageMissingRequiredParameter = 22,
        ExpiredTimestamp = 30,
        InvalidConsumerKey = 32,
        InvalidUsedNonce = 33,
        InvalidPermissions = 34,
        InvalidSignature = 35,
        InvalidExpiredToken = 36,
        UnsupportedSignatureMethod = 37,
        UnsupportedVersion = 38,
        ServiceUnavailable = 98,
        ReadOnlyMode = 99
    }
}
