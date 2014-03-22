// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResponse.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   LoginResponse represents data returned from a Facebook Login request
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FacebookLoginSimulator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    /// <summary>
    ///     LoginResponse represents data returned from a Facebook Login request
    /// </summary>
    public class LoginResponse
    {
        #region Constants

        /// <summary>
        ///     Defines the key for access token in the query string representation
        /// </summary>
        private const string AccessTokenKey = "access_token";

        /// <summary>
        ///     Defines the key for error code in the query string representation
        /// </summary>
        private const string ErrorCodeKey = "error_code";

        /// <summary>
        ///     Defines the key for error description in the query string representation
        /// </summary>
        private const string ErrorDescriptionKey = "error_description";

        /// <summary>
        ///     Defines the key for error in the query string representation
        /// </summary>
        private const string ErrorKey = "error";

        /// <summary>
        ///     Defines the key for error reason in the query string representation
        /// </summary>
        private const string ErrorReasonKey = "error_reason";

        /// <summary>
        ///     Defines the key for expires_in in the query string representation
        /// </summary>
        private const string ExpiresInKey = "expires_in";

        /// <summary>
        ///     Defines the format for a key value pair in the query string representation
        /// </summary>
        private const string PairFormat = "{0}={1}";

        /// <summary>
        ///     Defines the key for state in the query string representation
        /// </summary>
        private const string StateKey = "state";

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Gets or sets the error message
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        ///     Gets or sets the error code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Gets or sets the error description
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        ///     Gets or sets the error reason
        /// </summary>
        public string ErrorReason { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating when this token will expire
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the login response was successful and without any errors.
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.AccessToken) && (null != this.Expires) && string.IsNullOrEmpty(this.Error);
            }
        }

        /// <summary>
        ///     Gets or sets the state value
        /// </summary>
        public string State { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Converts the LoginResponse to a string representation
        /// </summary>
        /// <returns>String representation in query string format</returns>
        public override string ToString()
        {
            List<string> keyValuePairs;

            if (this.IsSuccess)
            {
                var expiresInSeconds = (long)(this.Expires - DateTime.UtcNow).TotalSeconds;
                keyValuePairs = new List<string> { BuildKeyValuePair(AccessTokenKey, this.AccessToken), BuildKeyValuePair(ExpiresInKey, expiresInSeconds.ToString()), BuildKeyValuePair(StateKey, this.State) };
            }
            else
            {
                keyValuePairs = new List<string> { BuildKeyValuePair(ErrorKey, this.Error), BuildKeyValuePair(ErrorCodeKey, this.ErrorCode.ToString()), BuildKeyValuePair(ErrorDescriptionKey, this.ErrorDescription), BuildKeyValuePair(ErrorReasonKey, this.ErrorReason), BuildKeyValuePair(StateKey, this.State) };
            }

            return null != keyValuePairs ? string.Join("&", keyValuePairs) : string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a key value pair for serialization to a query string
        /// </summary>
        /// <param name="key">
        /// Key in pair
        /// </param>
        /// <param name="value">
        /// Value in pair
        /// </param>
        /// <returns>
        /// Key value pair
        /// </returns>
        private static string BuildKeyValuePair(string key, string value)
        {
            return string.Format(PairFormat, HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
        }

        /// <summary>
        /// Gets a query string value from a full URI
        /// </summary>
        /// <param name="uri">
        /// URI to parse
        /// </param>
        /// <param name="key">
        /// QueryString key
        /// </param>
        /// <returns>
        /// QueryString value
        /// </returns>
        private string GetQueryStringValueFromUri(string uri, string key)
        {
            int questionMarkIndex = uri.LastIndexOf("?", StringComparison.Ordinal);
            if (questionMarkIndex == -1)
            {
                // if no questionmark found, this is just a queryString
                questionMarkIndex = 0;
            }

            string queryString = uri.Substring(questionMarkIndex, uri.Length - questionMarkIndex);
            queryString = queryString.Replace("#", string.Empty).Replace("?", string.Empty);

            string[] keyValuePairs = queryString.Split(new[] { '&' });
            for (int i = 0; i < keyValuePairs.Length; i++)
            {
                string[] pair = keyValuePairs[i].Split(new[] { '=' });
                if (pair[0].ToLowerInvariant() == key.ToLowerInvariant())
                {
                    return HttpUtility.UrlDecode(pair[1]);
                }
            }

            return string.Empty;
        }

        #endregion
    }
}