using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmugMug.Net.Data
{
    public class OAuthRequest
    {
        private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, 0);

        public readonly string Callback = "oob";
        public readonly string SignatureMethod = "HMAC-SHA1";
        public readonly string Version = "1.0";
        public string ConsumerKey = "";
        public string ConsumerSecret = "";
        public string Timestamp = "";
        public string Nonce = "";
        public string Signature = "";
        public string Token = "";
        public string TokenSecret = "";
        public string Verifier = "";

        public OAuthRequest()
        {
            this.Nonce = Guid.NewGuid().ToString();
            SetupTimestamp();
        }

        private void SetupTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - Epoch;
            this.Timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }
}
