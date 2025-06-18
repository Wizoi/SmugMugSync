using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Web;

namespace SmugMugCore.Net.Core
{
    /// <summary>
    /// Object which wraps functionality to provide input information for smug mug API queries
    /// </summary>
    public class QueryParameterList 
    {
        private readonly Dictionary<string, string> _values = new();

        /// <summary>
        /// Constructor
        /// </summary>
        public QueryParameterList()
        {
        }

        /// <summary>
        /// Copies the current parameter list into a new list
        /// </summary>
        /// <returns></returns>
        public QueryParameterList Clone()
        {
            var queryParamNew = new QueryParameterList();
            foreach (var key in _values.Keys)
            {
                queryParamNew.Add(key, _values[key]);
            }
            return queryParamNew;
        }

        /// <summary>
        /// Add a date/time value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, DateTime value)
        {
            this.Add(key, value.ToString("yyy-MM-dd hh:mm:ss.nnn"));
        }

        /// <summary>
        /// Add a float value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, long value)
        {
            this.Add(key, value.ToString());
        }

        /// <summary>
        /// Add a float value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, float value)
        {
            this.Add(key, value.ToString());
        }

        /// <summary>
        /// Add a decimal value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, decimal value)
        {
            this.Add(key, value.ToString());
        }

        /// <summary>
        /// Add a boolean value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        /// <param name="defaultValue">The default value for this item, if value = defaultValue, then it will not be added</param>
        public void Add(string key, bool value, bool defaultValue)
        {
            if (value == defaultValue)
            {
                // Ignore the empty value
            }
            else
            {
                this.Add(key, value);
            }

        }


        /// <summary>
        /// Add a boolean value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, bool value)
        {
            this.Add(key, value.ToString());
        }

        /// <summary>
        /// Add a integer value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        /// <param name="defaultValue">The default value for this item, if value = defaultValue, then it will not be added</param>
        public void Add(string key, int value, int defaultValue)
        {
            if (value == defaultValue)
            {
                // Ignore the empty value
            }
            else
            {
                this.Add(key, value);
            }

        }

        /// <summary>
        /// Add an integer value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, int value)
        {
            this.Add(key, value.ToString());
        }

        /// <summary>
        /// Add a string value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        /// <param name="defaultValue">The default value for this item, if value = defaultValue, then it will not be added</param>
        public void Add(string key, string value, string defaultValue)
        {
            if (value == defaultValue)
            {
                // Ignore the empty value
            }
            else
            {
                this.Add(key, value);
            }

        }

            /// <summary>
        /// Add a string value to the query parameter list
        /// </summary>
        /// <param name="key">Query Parameter</param>
        /// <param name="value">Value of the Query Parameter</param>
        public void Add(string key, string value)
        {
            if (value != null)
            {
                _values.Remove(key);
                _values.Add(key, value);
            }
        }

        /// <summary>
        /// Take all of the input values and output a query string by joining them together.
        /// </summary>
        public string GenerateQueryString(string queryPrefix)
        {
            string outVal;
            var paramGrouped = from x in _values.OrderBy(x => x, new QueryStringComparer())
                               where (x.Value ?? "").Length > 0
                               select x.Key + "=" + OAuthManager.UrlEncode(x.Value ?? "");

            if (paramGrouped.Any())
            {
                var paramString = string.Join("&", paramGrouped);
                outVal = queryPrefix + paramString;
            }
            else
            {
                outVal = "";
            }

            return outVal;
        }

        /// <summary>
        /// Output a dictionary object from the internal data.
        /// </summary>
        /// <returns></returns>
        public ICollection<KeyValuePair<String, String>> GenerateCollection()
        {
            var output = new Dictionary<string, string>(_values);
            return output;
        }

        /// <summary>
        /// Used to tell if this is empty
        /// </summary>
        /// <returns></returns>
        public bool HasEntries()
        {
            return (this._values.Count > 0);
        }

        /// <summary>
        /// Append all params to a header request
        /// </summary>
        /// <param name="request"></param>
        public void AppendToWebHeaders(HttpRequestMessage request)
        {
            var paramGrouped = from x in _values.OrderBy(x => x, new QueryStringComparer())
                               where (x.Value ?? "").Length > 0
                               select new {x.Key, Value=x.Value.Replace("\n","")};

            foreach (var param in paramGrouped)
            {
                request.Content?.Headers.Add(param.Key, param.Value);
            }
        }

        /// <summary>
        /// Per Oauth rules, parameters must be sorted A-Za-z; Capital params first, the lowercase params. 
        /// If the param matches, then the data is sorted.
        /// </summary>
        private class QueryStringComparer : IComparer<KeyValuePair<string, string>> 
        {
            public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y) 
            {
                if (x.Key == y.Key)
                {
                    return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
                }
                else
                {
                    return string.Compare(x.Key, y.Key, StringComparison.Ordinal);
                }
            }
        }
    }
}
