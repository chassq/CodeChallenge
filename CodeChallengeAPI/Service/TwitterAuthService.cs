using CodeChallengeAPI.Config;
using CodeChallengeAPI.Models;
using System.Text;
using System.Text.Json;

namespace CodeChallengeAPI.Service
{
    public interface ITwitterAuthService
    {
        string TwitterApplicationName { get; }

        Task<string> GetBearerToken();
    }

    /// <summary>
    /// Service provides Twitter Authentication services. For example making authentication token requests
    /// to the Twitter API.
    /// </summary>
    public class TwitterAuthService : ITwitterAuthService
    {
        protected readonly IConfiguration _configuration;

        private readonly TwitterAuthConfig _twitterAuthConfig;

        public string TwitterApplicationName { get; private set; }

        public TwitterAuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            _twitterAuthConfig = _configuration.GetSection(nameof(TwitterAuthConfig))?.Get<TwitterAuthConfig>()
                                            ?? throw new ArgumentNullException("The twitter authentication configuration is not set properly. Please see the readme.md file at the root of this project for instructions.");

            TwitterApplicationName = string.IsNullOrWhiteSpace(_twitterAuthConfig.ApplicationName) ? "" : _twitterAuthConfig.ApplicationName;
        }

        /// <summary>
        /// Method is used to return a oAuth application Bearer token from the Twitter API.
        /// <see cref="https://developer.twitter.com/en/docs/authentication/oauth-2-0/application-only"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetBearerToken()
        {
            if(!string.IsNullOrWhiteSpace(_twitterAuthConfig.BearerToken))
            {
                return _twitterAuthConfig.BearerToken;
            }

            var consumerKey = _twitterAuthConfig.ConsumerKey;
            var consumerSecret = _twitterAuthConfig.ConsumerSecret;
            var bearerTokenRequestEndpoint = _twitterAuthConfig.BearerTokenRequestEndpoint;

            //TODO - Investigate the sensitivity of exposing these values in a debug environment to a developer. If 
            //deemed too risky then this service could be moved out to a more isolated dedicated solution/project.
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey + ":" + consumerSecret));

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "SqCodeChallenge");

            var response = await httpClient.PostAsync(bearerTokenRequestEndpoint,
                new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var obj = JsonSerializer.Deserialize<TwitterAuthToken>(responseContent);

            if((obj == null) || string.IsNullOrWhiteSpace(obj.AccessToken))
            {
                throw new Exception("A bearer token could not be created for the request.");
            }

            return obj.AccessToken;

        }
    }
}
