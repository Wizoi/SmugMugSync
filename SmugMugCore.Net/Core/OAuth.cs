// OAuth/OAuth.cs
//
// Code to do OAuth stuff
//
// There's one main class: OAuth.Manager.  It handles interaction with the OAuth-
// enabled service, for requesting temporary tokens (aka request tokens), as well
// as access tokens. It also provides a convenient way to construct an oauth
// Authorization header for use in any Http transaction.
//
// -------------------------------------------------------
// Inital version by Dino Chiesa for Cropper
// Tue, 14 Dec 2010  12:31
// Changed in order to get it work with smugmug by Manuel Kaderli(suntsu@suntsu.ch)
// Tue, 29.12.2011
// Changed in order to get the sort ordering to work, and to get it to work in multi-threaded environments (Kevin Idzi)
// Tue, 21.02.2012
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace SmugMug.Net.Core
{
    /// <summary>
    ///   A class to manage OAuth 1.0A interactions. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This class holds the relevant oauth parameters, as well as
    ///     state for the oauth authentication dance.  This class also
    ///     exposes methods that communicate with the OAuth provider, or
    ///     generate elaborated quantities (like Authorization header
    ///     strings) based on all the oauth properties.
    ///   </para>
    /// </remarks>
    public class OAuthManager
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _token;
        private readonly string _tokenSecret;

        /// <summary>
        ///   The constructor to use when using OAuth when you already
        ///   have an OAuth access token.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The parameters for this constructor all have the
        ///     meaning you would expect.  The token and tokenSecret
        ///     are set in oauth_token, and oauth_token_secret.
        ///     These are *Access* tokens, obtained after a call
        ///     to AcquireAccessToken.  The application can store
        ///     those tokens and re-use them on successive runs.
        ///     the access tokens never expire.
        ///   </para>
        /// </remarks>
        /// <param name="consumerKey">The oauth_consumer_key parameter for OAuth</param>
        /// <param name="consumerSecret">The oauth_consumer_secret parameter for oauth.</param>
        /// <param name="token">The oauth_token parameter for oauth. This is sometimes called the Access Token.</param>
        /// <param name="tokenSecret">The oauth_token_secret parameter for oauth. This is sometimes called the Access Token Secret.</param>
        public OAuthManager(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = token;
            _tokenSecret = tokenSecret;
        }

        /// <summary>
        ///   The constructor to use when using OAuth when you already
        ///   have an OAuth consumer key and sercret, but need to
        ///   acquire an oauth access token.
        /// </summary>
        /// <param name="consumerKey">The oauth_consumer_key parameter for oauth. Get this, along with the consumerSecret</param>
        /// <param name="consumerSecret">The oauth_consumer_secret parameter for oauth.</param>
        public OAuthManager(string consumerKey, string consumerSecret)
            : this(consumerKey, consumerSecret, "", "") 
        { }


        /// <summary>
        ///   This method performs oauth-compliant Url Encoding.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This class provides an OAuth-friendly URL encoder.  .NET includes
        ///     a Url encoder in the base class library; see <see
        ///     href='http://msdn.microsoft.com/en-us/library/zttxte6w(v=VS.90).aspx'>
        ///     HttpServerUtility.UrlEncode</see>. But that encoder is not
        ///     sufficient for use with OAuth.
        ///   </para>
        ///   <para>
        ///     The builtin encoder emits the percent encoding in lower case,
        ///     which works for HTTP purposes, as described in the latest HTTP
        ///     specification (see <see
        ///     href="http://tools.ietf.org/html/rfc3986">RFC 3986</see>). But the
        ///     Oauth specification, provided in <see
        ///     href="http://tools.ietf.org/html/rfc5849">RFC 5849</see>, requires
        ///     that the encoding characters be upper case throughout OAuth.
        ///   </para>
        ///   <para>
        ///     For example, if you try to post a tweet message that includes a
        ///     forward slash, the slash needs to be encoded as %2F, and the
        ///     second hex digit needs to be uppercase.
        ///   </para>
        ///   <para>
        ///     It's not enough to simply convert the entire message to uppercase,
        ///     because that would of course convert un-encoded characters to
        ///     uppercase as well, which is undesirable.  This class provides an
        ///     OAuth-friendly encoder to do the right thing.
        ///   </para>
        /// </remarks>
        ///
        /// <param name="value">The value to encode</param>
        /// <returns>the Url-encoded version of that string</returns>
        public static string UrlEncode(string value)
        {
            const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            var encodedValue = new StringBuilder();
            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                    encodedValue.Append(symbol);
                else
                {
                    // encode it
                    encodedValue.Append('%');
                    var bytes = Encoding.UTF8.GetBytes(new char[] { symbol });

                    for (int j = 0; j < bytes.Length; j++)
                    {
                        encodedValue.Append(bytes[0].ToString("X2"));
                    }
                }
            }
            return encodedValue.ToString();
        }

        /// <summary>
        /// Formats the list of request parameters into string a according
        /// to the requirements of oauth. The resulting string could be used
        /// in the Authorization header of the request.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     There are 2 formats for specifying the list of oauth
        ///     parameters in the oauth spec: one suitable for signing, and
        ///     the other suitable for use within Authorization HTTP Headers.
        ///     This method emits a string suitable for the latter.
        ///   </para>
        /// </remarks>
        ///
        /// <param name="paramList">The list of parameters which to pull out the oauth params</param>
        /// <returns>a string representing the parameters</returns>
        private static string EncodeRequestHeaderParameters(QueryParameterList paramList)
        {
            var p = paramList.GenerateCollection();
            var sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.Where(x => x.Key.StartsWith("oauth_")).OrderBy(x => x.Key))
            {
                if (!String.IsNullOrEmpty(item.Value) &&
                    !item.Key.EndsWith("secret"))
                    sb.AppendFormat("{0}=\"{1}\", ",
                                    item.Key,
                                    UrlEncode(item.Value));
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }


        /// <summary>
        ///   Acquire a request token from the given URI using the given
        ///   HTTP method.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     To use this method, first instantiate a new Oauth.Manager
        ///     object, then, optionally, set the callback param
        ///     (oauth["callback"]='oob'). Consult the documentation
        ///     for the meaning and usage of the callback parameter.
        ///     After the call returns, you should direct the user to open a browser
        ///     window to the authorization page for the OAuth-enabled
        ///     service. Or, you can automatically open that page yourself. Do
        ///     this with System.Diagnostics.Process.Start(), passing the URL
        ///     of the page. 
        ///   </para>
        ///   <para>
        ///     According to the OAuth spec, you need to do this only ONCE per
        ///     application, the first time the application is run.  The
        ///     normal oauth workflow is: (1) get a request token, (2) use
        ///     that to acquire an access token (which requires explicit user
        ///     approval), then (3) using that access token, invoke protected
        ///     services. The first two steps need to be done only once, ever,
        ///     for each registered application. The third step can be
        ///     performed many times, over many invocations of the
        ///     application.
        ///   </para>
        /// </remarks>
        ///
        /// <seealso cref='AcquireAccessToken(string,string,string)'/>
        /// <seealso cref='AcquireAccessToken(string)'/>
        ///
        /// <example>
        ///   <para>
        ///     This example shows how to request an access token and key
        ///     It presumes you've already obtained a
        ///     consumer key and secret via app registration. Requesting
        ///     an access token is necessary only the first time you
        ///     contact the service. You can cache the access key and
        ///     token for subsequent runs, later.
        ///   </para>
        /// </example>
        ///
        /// <param name="uri">The uri to the "request token" endpoint of the service that implements oauth.   </param>
        /// <param name="method">The method you will use to send the message asking for a request token. </param> 
        /// <param name="oauthParams">Parameters for the request</param>
        /// <returns>A request object with the token and secret filled in.</returns>
        public async Task<Data.OAuthRequest> AcquireRequestToken(System.Uri uri, HttpMethod method, QueryParameterList oauthParams)
        {
            var oauthRequest = new Data.OAuthRequest { 
                ConsumerKey = _consumerKey, 
                ConsumerSecret = _consumerSecret
            };

            // Note this will append the oauth tokens to the request
            return await QueryWebsiteForOauthRequest(uri, method, oauthRequest, oauthParams);
        }

        /// <summary>
        ///   Acquire an access token, from the given URI, using the given
        ///   HTTP method.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     To use this method, you must first set the oauth_token to the value
        ///     of the request token.  Eg, oauth["token"] = "whatever".
        ///   </para>
        ///   <para>
        ///     According to the OAuth spec, you need to do this only ONCE per
        ///     application.  In other words, the first time the application
        ///     is run.  The normal oauth workflow is:  (1) get a request token,
        ///     (2) use that to acquire an access token (which requires explicit
        ///     user approval), then (3) using that access token, invoke
        ///     protected services.  The first two steps need to be done only
        ///     once per application.
        ///   </para>
        ///   <para>
        ///     you can cache the access tokens
        ///     indefinitely;  However, other
        ///     oauth services may not do the same. Also: the user may at any
        ///     time revoke his authorization for your app, in which case you
        ///     need to perform the first 2 steps again.
        ///   </para>
        /// </remarks>
        /// <seealso cref='AcquireRequestToken()'/>
        /// <param name="uri">The uri to the "access token" endpoint of the service that implements oauth.  </param>
        /// <param name="method">The method you will use to send the message asking for an access token. </param>
        /// <param name="pin">The PIN returned by the "Application approval" page shown </param>
        /// <param name="oauthParams">Parameters for the request</param>
        /// <returns>A request object with the token and secret filled in.</returns>
        public async Task<Data.OAuthRequest> AcquireAccessToken(System.Uri uri, HttpMethod method, string pin, QueryParameterList oauthParams)
        {
            var oauthRequest = new Data.OAuthRequest
            {
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                Token = _token,
                TokenSecret = _tokenSecret,
                Verifier = pin
            };

            // Note this will append the oauth tokens to the request
            return await QueryWebsiteForOauthRequest(uri, method, oauthRequest, oauthParams);
        }

        /// <summary>
        /// Queries the target website with the auth header to get valid oauth  tokens
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="oauthRequest"></param>
        /// <param name="oauthParams"></param>
        /// <returns></returns>
        public async static Task<Data.OAuthRequest> QueryWebsiteForOauthRequest(System.Uri uri, HttpMethod method, Data.OAuthRequest oauthRequest, QueryParameterList oauthParams)
        {
            using HttpClient httpClient = new(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });
            using HttpRequestMessage request = new(method, uri);

            request.Version = HttpVersion.Version30;
            var authzHeader = GetAuthorizationHeader(oauthRequest, uri, method, "", oauthParams);
            httpClient.DefaultRequestHeaders.Add("Authorization", authzHeader);
            var response = await httpClient.SendAsync(request);

            using var reader = new System.IO.StreamReader(await response.Content.ReadAsStreamAsync());
            var r = new OAuthResponse(await reader.ReadToEndAsync());

            // Update the token with the new values from the response (if they exist)
            oauthRequest.Token = r["oauth_token"];
            if (r["oauth_token_secret"] != null)
                oauthRequest.TokenSecret = r["oauth_token_secret"];

            return oauthRequest;
        }


        /// <summary>
        ///   Generate a string to be used in an Authorization header in
        ///   an HTTP request.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method assembles the available oauth_ parameters that
        ///     have been set in the Dictionary in this instance, produces
        ///     the signature base (As described by the OAuth spec, RFC 5849),
        ///     signs it, then re-formats the oauth_ parameters into the
        ///     appropriate form, including the oauth_signature value, and
        ///     returns the result.
        ///   </para>
        /// </remarks>
        /// <seealso cref='GenerateCredsHeader'/>
        ///
        /// <param name="uri">The target URI that the application will connet to, via an OAuth-protected protocol. </param>
        /// <param name="method">The HTTP method that will be used to connect to the target URI. </param>
        /// <param name="paramList">Parameters for the request</param>
        /// <returns>The OAuth authorization header that has been generated given all the oauth input parameters.</returns>
        public string GenerateAuthzHeader(System.Uri uri, HttpMethod method, QueryParameterList? paramList)
        {
            var oauthRequest = new Data.OAuthRequest
            {
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                Token = _token,
                TokenSecret = _tokenSecret
            };
            var authzHeader = GetAuthorizationHeader(oauthRequest, uri, method, null, paramList);
            return authzHeader;
        }

        /// <summary>
        /// Private method which actually will get the authorization header
        /// </summary>
        /// <param name="oauthReq">Request object with oauth params</param>
        /// <param name="uri">The uri to the "access token" endpoint of the service that implements oauth.  </param>
        /// <param name="method">The method you will use to send the message asking for an access token. </param>
        /// <param name="realm"></param>
        /// <param name="paramList">Parameters for the request</param>
        /// <returns>OAuth Header String</returns>
        private static string GetAuthorizationHeader(Data.OAuthRequest oauthReq, System.Uri uri, HttpMethod method, string? realm, QueryParameterList? paramList)
        {
            var oauthParams = (paramList ?? new QueryParameterList()).Clone();

            oauthParams.Add("oauth_callback", UrlEncode(oauthReq.Callback), "");
            oauthParams.Add("oauth_verifier", oauthReq.Verifier ?? "", "");
            oauthParams.Add("oauth_signature_method", oauthReq.SignatureMethod);
            oauthParams.Add("oauth_version", oauthReq.Version);
            oauthParams.Add("oauth_consumer_key", oauthReq.ConsumerKey);
            oauthParams.Add("oauth_timestamp", oauthReq.Timestamp);
            oauthParams.Add("oauth_nonce", oauthReq.Nonce);
            oauthParams.Add("oauth_token", oauthReq.Token);

            // Find the Signature, do not have the signature or secrets in the query params
            var signatureBase = GetSignatureBase(uri, method, oauthParams);
            oauthReq.Signature = Sign(signatureBase, oauthReq);

            // Append the Signature and secrets to the param list
            oauthParams.Add("oauth_signature", oauthReq.Signature);

            var erp = EncodeRequestHeaderParameters(oauthParams);
            return (String.IsNullOrEmpty(realm))
                ? "OAuth " + erp
                : String.Format("OAuth realm=\"{0}\", ", realm) + erp;
        }

        /// <summary>
        /// Formats the list of request parameters into "signature base" string as
        /// defined by RFC 5849.  This will then be MAC'd with a suitable hash.
        /// </summary>
        private static string GetSignatureBase(System.Uri uri, HttpMethod method, QueryParameterList oauthParams)
        {
            // normalize the URI
            var normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // The parameters follow. This must include all oauth params
            // plus any query params on the uri.  Also, each uri may
            // have a distinct set of query params.
            string queryParamString = UrlEncode(oauthParams.GenerateQueryString(""));

            var signatureBase = string.Format("{0}&{1}&{2}", method.Method, UrlEncode(normUrl), queryParamString);
            return signatureBase;
        }

        /// <summary>
        /// Signs the signature base and returns the hash string
        /// </summary>
        /// <param name="signatureBase"></param>
        /// <param name="oauthReq"></param>
        /// <returns></returns>
        private static string Sign(string signatureBase, Data.OAuthRequest oauthReq)
        {
            // We only support HMAC-SHA1 Signature Methods
            if (oauthReq.SignatureMethod != "HMAC-SHA1")
                throw new NotImplementedException();

            // Build the key using the secrets
            var hash = new HMACSHA1();
            var keystring = string.Format("{0}&{1}", UrlEncode(oauthReq.ConsumerSecret), UrlEncode(oauthReq.TokenSecret));
            hash.Key = Encoding.UTF8.GetBytes(keystring);

            // Compute the hash using the signature base
            var dataBuffer = System.Text.Encoding.UTF8.GetBytes(signatureBase);
            var hashBytes = hash.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
    }


    /// <summary>
    ///   A class to hold an OAuth response message.
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        ///   All of the text in the response. This is useful if the app wants
        ///   to do its own parsing.
        /// </summary>
        public string AllText { get; set; }
        private readonly Dictionary<String, String> _params;

        /// <summary>
        ///   a Dictionary of response parameters.
        /// </summary>
        public string this[string ix]
        {
            get
            {
                return _params[ix];
            }
        }

        /// <summary>
        ///   Constructor for the response to one transmission in an oauth dialogue.
        ///   An application or may not not want direct access to this response.
        /// </summary>
        internal OAuthResponse(string alltext)
        {
            AllText = alltext;
            _params = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                _params.Add(kv[0], kv[1]);
            }
            // expected keys:
            //   oauth_token, oauth_token_secret, user_id, screen_name, etc
        }
    }
}

