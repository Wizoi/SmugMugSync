using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using SmugMug.Net.Core;
using SmugMug.Net.Service;

namespace SmugMug.Net.Core
{
    /// <summary>
    /// Base class which provides Authentication and core querying logic
    /// </summary>
    public class SmugMugCore
    {
        private readonly string _smugMugApiKey;
        private readonly string _smugMugSecret;
        private OAuthManager? _oauthManager = null;
        public bool EnableRequestLogging { get; set; }


        #region Properties for Services
        private readonly System.Collections.Generic.Dictionary<string, object> _serviceCatalog = new();

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual AccountService AccountService 
        {
            get { 
                string keyName = "AccountService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new AccountService(this)); }
                return (AccountService) _serviceCatalog[keyName];
            }
        }

        public virtual AlbumService AlbumService
        {
            get
            {
                string keyName = "AlbumService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new AlbumService(this)); }
                return (AlbumService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual AlbumTemplateService AlbumTemplateService
        {
            get
            {
                string keyName = "AlbumTemplateService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new AlbumTemplateService(this)); }
                return (AlbumTemplateService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual CategoryService CategoryService
        {
            get
            {
                string keyName = "CategoryService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new CategoryService(this)); }
                return (CategoryService)_serviceCatalog[keyName];
            }
        }

        public virtual CommunityService CommunityService
        {
            get
            {
                string keyName = "CommunityService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new CommunityService(this)); }
                return (CommunityService)_serviceCatalog[keyName];
            }
        }

        public virtual ContentMetadataService ContentMetadataService
        {
            get
            {
                string keyName = "ContentMetadataService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new ContentMetadataService()); }
                return (ContentMetadataService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual CouponService CouponService
        {
            get
            {
                string keyName = "CouponService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new CouponService(this)); }
                return (CouponService)_serviceCatalog[keyName];
            }
        }

        public virtual FamilyService FamilyService
        {
            get
            {
                string keyName = "FamilyService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new FamilyService(this)); }
                return (FamilyService)_serviceCatalog[keyName];
            }
        }
        public virtual FanService FanService
        {
            get
            {
                string keyName = "FanService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new FanService(this)); }
                return (FanService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual FeaturedAlbumService FeaturedAlbumService
        {
            get
            {
                string keyName = "FeaturedAlbumService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new FeaturedAlbumService(this)); }
                return (FeaturedAlbumService)_serviceCatalog[keyName];
            }
        }

        public virtual FriendService FriendService
        {
            get
            {
                string keyName = "FriendService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new FriendService(this)); }
                return (FriendService)_serviceCatalog[keyName];
            }
        }

        public virtual ImageService ImageService
        {
            get
            {
                string keyName = "ImageService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new ImageService(this)); }
                return (ImageService)_serviceCatalog[keyName];
            }
        }
        public virtual ImageUploaderService ImageUploaderService
        {
            get
            {
                string keyName = "ImageUploaderService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new ImageUploaderService(this)); }
                return (ImageUploaderService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual PrintmarkService PrintmarkService
        {
            get
            {
                string keyName = "PrintmarkService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new PrintmarkService(this)); }
                return (PrintmarkService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual ShareGroupService ShareGroupService
        {
            get
            {
                string keyName = "ShareGroupService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new ShareGroupService(this)); }
                return (ShareGroupService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual StyleService StyleService
        {
            get
            {
                string keyName = "StyleService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new StyleService(this)); }
                return (StyleService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual SubCategoryService SubCategoryService
        {
            get
            {
                string keyName = "SubCategoryService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new SubCategoryService(this)); }
                return (SubCategoryService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual ThemeService ThemeService
        {
            get
            {
                string keyName = "ThemeService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new ThemeService(this)); }
                return (ThemeService)_serviceCatalog[keyName];
            }
        }

        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual UserService UserService
        {
            get
            {
                string keyName = "UserService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new UserService(this)); }
                return (UserService)_serviceCatalog[keyName];
            }
        }
        [Obsolete("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
        public virtual WatermarkService WatermarkService
        {
            get
            {
                string keyName = "WatermarkService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.Add(keyName, new WatermarkService(this)); }
                return (WatermarkService)_serviceCatalog[keyName];
            }
        }

        /// <summary>
        /// Validates a NiceName Parameter
        /// http://stackoverflow.com/questions/2063213/regular-expression-for-validating-dns-label-host-name
        /// </summary>  
        /// <param name="niceName"></param>
        /// <returns></returns>
        public static bool IsValidNiceName(string niceName)
        {
            string niceNamePattern = "^[a-zA-Z0-9]+[a-zA-Z0-9-]*[a-zA-Z0-9]+$";
            var result = System.Text.RegularExpressions.Regex.IsMatch(niceName, niceNamePattern);
            return result;
        }

        #endregion Properties for Services

        /// <summary>
        /// Instantiate an unauthenticated Core library
        /// </summary>
        public SmugMugCore(string apiKey, string apiSecret)
        {
            this.EnableRequestLogging = false;
            this._smugMugSecret = apiSecret;
            this._smugMugApiKey = apiKey;
        }

        /// <summary>
        /// Instantiate the object and authenticate
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <param name="userAuthSecret"></param>
        public SmugMugCore(string userAuthToken, string userAuthSecret, string  apiKey, string apiSecret) : this(apiKey, apiSecret)
        {
            Authenticate(userAuthToken, userAuthSecret);
        }

        /// <summary>
        /// Authenticate the user with their secret
        /// </summary>
        /// <param name="userAuthToken">User Authentication Token</param>
        /// <param name="userAuthSecret">User Authentication Secret</param>
        internal void Authenticate(string userAuthToken, string userAuthSecret)
        {
            _oauthManager = new OAuthManager(_smugMugApiKey, _smugMugSecret, userAuthToken, userAuthSecret);
        }

        /// <summary>
        /// Ping the service, throw an exception if there are problems
        /// </summary>
        /// <returns>True if service is pingable</returns>
        public async Task<bool> PingService()
        {
            var paramList = new Core.QueryParameterList();
            paramList.Add("APIKey", _smugMugApiKey);

            _ = await QueryWebsite<Data.SmugmugError>("smugmug.service.ping", paramList, false);
            return true;
        }

        /// <summary>
        /// Download a SmugMug image or media file from the web 
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="localPath"></param>
        internal async Task<Boolean> DownloadContentAsync(string targetUrl, string localPath)
        {
            var paramData = new QueryParameterList();            var uri = new System.Uri(targetUrl);
            var httpResponse = await RetrieveHttpRequestForGetAsync(uri, paramData, new TimeSpan(0, 5, 0));

            System.IO.Stream? stream = null;
            try
            {
                stream = await httpResponse.Content.ReadAsStreamAsync();
                var cancelToken = CancellationToken.None;
                byte[] read = new byte[256];
                using (FileStream fs = new(localPath, FileMode.Create))
                {
                    int count = await stream.ReadAsync(read, cancelToken);
                    while (count > 0)
                    {
                        await fs.WriteAsync(read.AsMemory(0, count), cancelToken);
                        count = await stream.ReadAsync(read, cancelToken);
                    }
                    fs.Close();
                }
                stream.Close();
            }
            finally
            {
                stream?.Dispose();
            }

            return true;
        }

        /// <summary>
        /// General method used to make requests against SmugMug API
        /// </summary>
        /// <param name="methodName">Name of the SmugMug API to Invoke</param>
        /// <param name="paramData">Parameters to pass into API</param>
        /// <returns></returns>
        internal async Task<T[]> QueryWebsite<T>(string methodName, QueryParameterList paramData, bool returnArray)
        {
            bool allowDebuggingOutput = this.EnableRequestLogging;
            string baseUri = "https://api.smugmug.com/services/api/rest/1.3.0/";
            string method = WebRequestMethods.Http.Get;
            string? requestDetail = null;

            // Take the parameters and package them up into the GET request
            paramData ??= new QueryParameterList();

            paramData.Add("method", methodName);
            var uri = new System.Uri(baseUri);
            var httpResponse = await RetrieveHttpRequestForGetAsync(uri, paramData, new TimeSpan(0, 0, 30));

            System.IO.Stream? stream = null;
            try
            {
                // If debugging, allow a second req to pull the XML for validation
                if (allowDebuggingOutput)
                {
                    var preStream = await httpResponse.Content.ReadAsStreamAsync();
                    var streamReader = new System.IO.StreamReader(preStream);
                    var xmlResponse = streamReader.ReadToEnd();

                    // Response streams cannot be re-positioned, so we reset the stream for the XML Serialization
                    stream = new System.IO.MemoryStream();
                    byte[] data = Encoding.UTF8.GetBytes(xmlResponse);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;

                    var sbRequestDetail = new StringBuilder();
                    sbRequestDetail.AppendLine("API Name: " + methodName);
                    sbRequestDetail.AppendLine("Method: " + method);
                    sbRequestDetail.AppendLine("Base URL: " + uri.PathAndQuery);
                    sbRequestDetail.AppendLine("Query String: " + paramData.GenerateQueryString("?"));
                    if (httpResponse.RequestMessage != null && httpResponse.RequestMessage.Headers != null)
                    {
                        if (httpResponse.RequestMessage.Headers.Any())
                        {
                            sbRequestDetail.AppendLine("Headers: ");
                            foreach (var header in httpResponse.RequestMessage.Headers)
                            {
                                sbRequestDetail.AppendLine("\t" + header.Key + ": " + header.Value);
                            }
                        }
                    }
                    sbRequestDetail.AppendLine();
                    sbRequestDetail.AppendLine("Response: ");
                    sbRequestDetail.AppendLine(xmlResponse);
                    requestDetail = sbRequestDetail.ToString();

                }
                else
                {
                    stream = await httpResponse.Content.ReadAsStreamAsync();
                    requestDetail = string.Empty;
                }

                if (httpResponse.Content != null && httpResponse.Content.Headers != null)
                {
                    if (httpResponse.Content.Headers.ContentType != null && httpResponse.Content.Headers.ContentType.ToString().Contains("text/xml"))
                    {
                        using var reader = new System.IO.StreamReader(stream);
                        var dataResults = DeserializeResponse<T>(reader, returnArray, methodName, paramData, requestDetail);
                        return dataResults.ToArray();
                    }
                    else if (httpResponse.Content.Headers.ContentLocation != null)
                    {
                        // Content length is -1, so we look at the response.uri that was sent (redirection)
                        T responseUri = (T)((object)httpResponse.Content.Headers.ContentLocation.AbsoluteUri);
                        var dataResults = new List<T>
                            {
                                responseUri
                            };
                        return dataResults.ToArray();
                    }
                }
            }
            finally
            {
                stream?.Dispose();
                httpResponse.Dispose();
            }

            throw new ApplicationException("Results not found from response.");
        }

        /// <summary>
        /// Take the content in a reader and return an array of multiple (or array of one) element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="returnArray"></param>
        /// <param name="methodName">Method name for error handling</param>
        /// <param name="paramData">PramData for error handling</param>
        /// <returns></returns>
        internal static List<T> DeserializeResponse<T>(
            System.IO.StreamReader reader, bool returnArray, string methodName, QueryParameterList paramData, string requestDetail)
        {
            string outputData = reader.ReadToEnd();
            StringReader sr = new(outputData);
            var xmlReader = System.Xml.XmlReader.Create(sr);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    if (xmlReader.Name == "rsp")
                    {
                        if (xmlReader.MoveToFirstAttribute())
                        {
                            if (xmlReader.Value != "ok")
                            {
                                // Do some error handling
                            }
                        }

                    }
                    else if (xmlReader.Name == "method")
                    {
                        // Ignore this
                        xmlReader.Skip();
                        break;
                    }
                }
            }

            // If an array is to be returned, then skip the parent node which just designtes that the child elements are returned
            // so when we start looping, it is ove the child elements.
            if (returnArray && (xmlReader.Name != "err"))
            {
                xmlReader.Read();
            }

            var dataResults = new List<T>();
            do
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    var xmlSubtreeReader = xmlReader.ReadSubtree();

                    // Serialize the results
                    if (xmlReader.Name == "err")
                    {
                        var serializer = new XmlSerializer(typeof(Data.SmugmugError));
                        var errorResponse = serializer.Deserialize(xmlSubtreeReader) as Data.SmugmugError ?? throw new ApplicationException("Parsing error deserializing error response.");
                        SmugMugCore.ProcessError(errorResponse, methodName, paramData, requestDetail);
                    }
                    else
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        if (serializer != null)
                        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                            var queryResponse = (T)serializer.Deserialize(xmlSubtreeReader) ?? throw new ApplicationException("Parsing error deserializing valid response.");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                            dataResults.Add(queryResponse);
                        }
                        else
                            throw new ApplicationException("Serializer  for current type being processed is not managed.");
                    }
                }
                xmlReader.Read();
            } while ((xmlReader.NodeType != System.Xml.XmlNodeType.EndElement) && (!xmlReader.EOF));
            return dataResults;
        }

        /// <summary>
        /// IF an error occurs, a try/catch will be thrown. There will be extra fields on it to allow filtering at the client.
        /// </summary>
        /// <param name="errorResponse">Error Response object from SmugMug</param>
        /// <param name="methodName">Name of the method which contained the error</param>
        /// <param name="paramData">Parameters provided to SmugMug which generated the error</param>
        private static void ProcessError(Data.SmugmugError errorResponse, string methodName, QueryParameterList paramData, string requestDetail)
        {
            throw new SmugMugException(errorResponse, methodName, paramData, requestDetail);
        }

        /// <summary>
        /// Create the Request Message to use for HTTP requests
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="method"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "API Simplicity")]
        internal HttpRequestMessage BuildHttpRequestMessage(System.Uri baseUri, HttpMethod method, QueryParameterList? paramData)
        {
            // To get the authorization string, the full path + Params must be evaluated
            string authUri = baseUri.AbsoluteUri;
            if (paramData != null)
                authUri += paramData.GenerateQueryString("?");

            var request = new HttpRequestMessage(method, authUri);
            request.Version = HttpVersion.Version30;
            return request;
        }

        /// <summary>
        /// Builds an HTTP Client with  the  proper authz header already in place
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="method"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        internal HttpClient BuildHttpClient(System.Uri baseUri, HttpMethod method, QueryParameterList? paramData)
        {
            if (_oauthManager == null)
                throw new ApplicationException("oauthmanager is null, and should be setup");

            HttpClient httpClient = new(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });

            string authzHeader = _oauthManager.GenerateAuthzHeader(new System.Uri(baseUri.AbsoluteUri), method, paramData);
            httpClient.DefaultRequestHeaders.Add("Authorization", authzHeader);

            return httpClient;
        }

        /// <summary>
        /// Internal method to let services construct authorization headers if needed
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal async Task<HttpResponseMessage> RetrieveHttpRequestForGetAsync(System.Uri baseUri, QueryParameterList? paramData, System.TimeSpan requestTimeout)
        {
            HttpClient? client = null;
            HttpRequestMessage? request = null;
            HttpResponseMessage? response = null;

            try
            {
                client = this.BuildHttpClient(baseUri, HttpMethod.Get, paramData);
                client.Timeout = requestTimeout;

                request = this.BuildHttpRequestMessage(baseUri, HttpMethod.Get, paramData);
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            finally
            {
                client?.Dispose();
                request?.Dispose();
            }
            return response;
        }

        /// <summary>
        /// Return the fields with XML Mapping for a data object
        /// </summary>
        /// <param name="targetClass"></param>
        /// <returns></returns>
        public static string[] GetFields(Type targetClass)
        {
            var classFields = targetClass.GetFields();
            var fieldsWithXmlAttributes = classFields.Where(x => (x.GetCustomAttributes(typeof(XmlAttributeAttribute), false).Length > 0));
            var fieldNames = fieldsWithXmlAttributes.Select(x => x.Name);
            return fieldNames.ToArray();
        }

        /// <summary>
        /// Take a list of field names and return the Xml Field Mappings
        /// </summary>
        /// <param name="targetClass"></param>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        internal static string[] TranslateFieldToAttribute(Type targetClass, string[] fieldList)
        {
            var classFields = from x in targetClass.GetFields()
                              join y in fieldList
                              on x.Name equals y
                              select new
                              {
                                  FieldData = x.GetCustomAttributes(typeof(XmlAttributeAttribute), false) as XmlAttributeAttribute[],
                                  FieldName = x.Name
                              };

            var classFieldAttributes = from x in classFields
                                       where x.FieldData.Length == 1
                                       select new
                                       {
                                           x.FieldName,
                                           XmlMappingName = (string)x.FieldData.First().AttributeName
                                       };

            return classFieldAttributes.Select(x => x.XmlMappingName).ToArray();
        }


        /// <summary>
        /// Helper method to convert a list of fields to the xml mapping names
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        internal static string? ConvertFieldListToXmlFields<T>(string[] fieldList)
        {
            if (fieldList != null)
            {
                // Convert the field list coming in to a comma list of fields the webservice can interpret 
                var xmlMapping = Core.SmugMugCore.TranslateFieldToAttribute(typeof(T), fieldList);
                var xmlList = String.Join(",", xmlMapping);
                return xmlList;
            }
            else
            {
                return null;
            }
        }

    }
}
