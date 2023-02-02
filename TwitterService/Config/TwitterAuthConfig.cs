namespace TwitterService.Config
{
    /// <summary>
    /// This class is used to represent Twitter authentication configuration information. The values for this class derive 
    /// from the setup of a Twitter developer account <see cref="https://developer.twitter.com/"/> and creation of an application in the developer account. 
    /// This class should be instantiated from a secured configuration store (e.g. Azure App Configuration or Azure Key Vault) or 
    /// other securable location. These values should NOT come from appsettings.json or other location where they can be readily 
    /// seen in clear text as they are sensitive.
    /// </summary>
    public class TwitterAuthConfig
    {
        /// <summary>
        /// Twitter developer account application name
        /// </summary>
        public string? BearerTokenRequestEndpoint { get; set; }

        /// <summary>
        /// Twitter developer account application name
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Twitter developer account application consumer key. This will be used as
        /// part of a basic auth request to receive an OAuth bearer token. 
        /// <see cref="https://developer.twitter.com/en/docs/authentication/oauth-1-0a/api-key-and-secret"/>
        /// </summary>
        public string? ConsumerKey {get; set; }

        /// <summary>
        /// Twitter developer account application consumer secret. This will be used as
        /// part of a basic auth request to receive an OAuth bearer token.
        /// <see cref="https://developer.twitter.com/en/docs/authentication/oauth-1-0a/api-key-and-secret"/>
        /// </summary>
        public string? ConsumerSecret {get; set; }

        /// <summary>
        /// Twitter developer account application bearer token. If the Bearer token is provided it will be used instead of the 
        /// Consumer key/secret combination. If the ConsumerKey and ConsumerSecret are used the resulting Bearer token should be the
        /// same value as the generated application Bearer token.
        /// <see cref="https://developer.twitter.com/en/docs/authentication/oauth-2-0/application-only"/>
        /// </summary>
        public string? BearerToken { get; set; }
    }
}
