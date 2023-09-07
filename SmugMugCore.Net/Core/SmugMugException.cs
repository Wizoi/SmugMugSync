using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace SmugMug.Net.Core
{
    /// <summary>
    /// General Application Exception to return smugmug error information
    /// </summary>
    public class SmugMugException: Exception
    {
        public SmugMugException()
        {
            this.ErrorResponse = new();
            this.ParamData = new();
        }
        /// <summary>
        /// Parameter Information
        /// </summary>
        public QueryParameterList ParamData;

        /// <summary>
        /// Error Response Object from SmugMug
        /// </summary>
        public Data.SmugmugError ErrorResponse;
        
        /// <summary>
        /// Method which generated the error
        /// </summary>
        public string MethodName = "";

        /// <summary>
        /// Querystring passed into method
        /// </summary>
        public string QueryString = "";

        /// <summary>
        /// Request Details (if enabled)
        /// </summary>
        public string RequestDetail = "";

        /// <summary>
        /// Creates a clean exception, and provides public properties for the method, querystring and error data
        /// </summary>
        /// <param name="errorResponse">Error Response object from SmugMug</param>
        /// <param name="methodName">Name of the method which contained the error</param>
        /// <param name="paramData">Parameters provided to SmugMug which generated the error</param>
        /// <param name="requestDetail">Request detail if logged</param>
        public SmugMugException(Data.SmugmugError errorResponse, string methodName, QueryParameterList paramData, string requestDetail)
            : base(string.Format("Error: {0} - {1} calling {2}", errorResponse.Code, errorResponse.Message, methodName))
        {
            this.ParamData = paramData;
            this.QueryString = paramData.GenerateQueryString("");
            if (requestDetail != null)
                this.RequestDetail = requestDetail;

            this.ErrorResponse = errorResponse;
            this.MethodName = methodName;

            base.Data["Code"] = errorResponse.Code;
            base.Data["Description"] = errorResponse.Message;
            base.Data["MethodName"] = methodName;
            base.Data["QueryString"] = this.QueryString;
        }
    }
}
