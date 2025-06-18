using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using RestSharp;

namespace SmugMugCore.Net.Core20
{
    /// <summary>
    /// General Application Exception to return smugmug error information
    /// </summary>
    public class SmugMugException: Exception
    {
        /// <summary>
        /// Method which generated the error
        /// </summary>
        public string MethodName = "";

        /// <summary>
        /// Error code generated
        /// </summary>
        public int ErrorCode;

        /// <summary>
        /// Error code generated
        /// </summary>
        public string ErrorMessage = "";

        /// <summary>
        /// Response details
        /// </summary>
        public string ResponseDetail = "";

        /// <summary>
        /// Request object sent
        /// </summary>
        public RestRequest RequestObject;

        /// <summary>
        /// Creates a clean exception, and provides public properties for the method, querystring and error data
        /// </summary>

        /// <param name="request">Request object used</param>
        /// <param name="responseText">Error Response object from SmugMug</param>
        /// <param name="method">Name of the method which contained the error</param>
        /// <param name="errorCode">Code from Smugmug for the specific error</param>
        /// <param name="errorMessage">Error Message</param>
        public SmugMugException(RestRequest request, string responseText, string method, int errorCode, string errorMessage)
             : base(string.Format("Error: {0} - {1} calling {2}", errorCode, errorMessage, method))
        {
            this.ResponseDetail = responseText;
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;
            this.MethodName = method;
            this.RequestObject = request;

            base.Data["Code"] = errorCode;
            base.Data["Description"] = errorMessage;
            base.Data["MethodName"] = method;
            base.Data["Response"] = responseText;
            base.Data["Request"] = request;
        }
    }
}
