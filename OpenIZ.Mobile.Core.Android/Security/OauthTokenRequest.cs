using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using OpenIZ.Core.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
    /// <summary>
    /// OAuth token response.
    /// </summary>
    [JsonObject]
    public class OAuthTokenResponse
    {

        /// <summary>
        /// Gets or sets the error
        /// </summary>
        [JsonProperty("error")]
        public String Error { get; set; }

        /// <summary>
        /// Description of the error
        /// </summary>
        [JsonProperty("error_description")]
        public String ErrorDescription { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        /// <summary>
        /// Expires in
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonProperty("refresh_token")]
        public String RefreshToken { get; set; }

        /// <summary>
        /// Represent the object as a string
        /// </summary>
        public override string ToString()
        {
            return string.Format("[OAuthTokenResponse: Error={0}, ErrorDescription={1}, AccessToken={2}, TokenType={3}, ExpiresIn={4}, RefreshToken={5}]", Error, ErrorDescription, AccessToken, TokenType, ExpiresIn, RefreshToken);
        }
    }

    /// <summary>
    /// OAuth token request.
    /// </summary>
    public class OAuthTokenRequest
    {

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="OpenIZ.Mobile.Core.Android.Security.OAuthTokenServiceCredentials+OAuthTokenRequest"/> class.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="scope">Scope.</param>
        public OAuthTokenRequest(String username, String password, String scope)
        {
            this.Username = username;
            this.Password = password;
            this.Scope = scope;
            this.GrantType = "password";
        }

        /// <summary>
        /// Token request for refresh
        /// </summary>
        public OAuthTokenRequest(TokenClaimsPrincipal current)
        {
            this.GrantType = "refresh_token";
            this.RefreshToken = current.RefreshToken;
        }

        /// <summary>
        /// Gets the type of the grant.
        /// </summary>
        /// <value>The type of the grant.</value>
        [FormElement("grant_type")]
        public String GrantType
        {
            get; set;
        }

        /// <summary>
        /// Gets the refresh token
        /// </summary>
        [FormElement("refresh_token")]
        public String RefreshToken { get; private set; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>The username.</value>
        [FormElement("username")]
        public String Username
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        [FormElement("password")]
        public String Password
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        [FormElement("scope")]
        public String Scope
        {
            get;
            private set;
        }

    }
}