using CodeChallengeAPI.Config;
using CodeChallengeAPI.Models;
using System.Text;
using System.Text.Json;

namespace CodeChallengeAPI.Service
{
    public interface ITwitterStreamService
    {
        Task StartStream();
        void StopStream();
        TweetSummary GetSummary();
    }

    public class TwitterStreamService : ITwitterStreamService
    {
        private readonly IServiceProvider _services;

        protected readonly IConfiguration _configuration;

        private readonly TwitterAuthConfig _twitterAuthConfig;

        private bool _streamIsActive = false;

        private long _tweetCounter = 0;

        private readonly List<TweetHashtag> _hashTagsStore = new();

        public TwitterStreamService(IServiceProvider services, IConfiguration configuration)
        {
            _services = services;

            _configuration = configuration;

            _twitterAuthConfig = _configuration.GetSection(nameof(TwitterAuthConfig))?.Get<TwitterAuthConfig>()
                                            ?? throw new ArgumentNullException("The twitter authentication configuration is not set properly. Please see the readme.md file at the root of this project for instructions.");
        }

        public async Task StartStream()
        {
            _ = Task.Run(async () =>
            {
                using var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var logger = scope.ServiceProvider.GetService<ILogger>();

                try
                {
                    // Create a new HttpClient
                    var httpClient = new HttpClient();

                    httpClient.DefaultRequestHeaders.Add("User-Agent", _twitterAuthConfig.ApplicationName);

                    var url = "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=entities&expansions=author_id&user.fields=created_at";

                    var bearerToken = await GetBearerToken();

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();

                    _streamIsActive = true;

                    using var reader = new StreamReader(stream); 

                    while (_streamIsActive)
                    {
                        var line = await reader.ReadLineAsync();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var tweet = JsonSerializer.Deserialize<Tweet>(line);

                            if(tweet != null) _tweetCounter++;

                            var hashtags = tweet?.TweetData?.TweetEntity?.TweetHashTags;
                            if (hashtags != null && hashtags.Any())
                            {    
                                foreach( var hashtag in hashtags)
                                {
                                    if (_hashTagsStore.Any(t => !string.IsNullOrWhiteSpace(t.Tag) && t.Tag.Equals(hashtag.Tag, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        var updateTag = _hashTagsStore.Where(t => !string.IsNullOrWhiteSpace(t.Tag) && t.Tag.Equals(hashtag.Tag, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                                        if(updateTag != null)
                                        {
                                            updateTag.Count++;
                                        }
                                    }
                                    else
                                    {
                                        hashtag.Count++;
                                        _hashTagsStore.Add(hashtag);
                                    }
                                }                                
                            }                            
                        }                                               
                    }
                }
                catch (Exception ex)
                {
                    _streamIsActive = false;
                    logger.LogError(ex, ex.Message);
                }
            });

            await Task.Delay(2000).ConfigureAwait(false);
        }

        async Task<string> GetBearerToken()
        {
            var consumerKey = _twitterAuthConfig.ConsumerKey;
            var consumerSecret = _twitterAuthConfig.ConsumerSecret;
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey + ":" + consumerSecret));
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "SqCodeChallenge");

            var response = await httpClient.PostAsync("https://api.twitter.com/oauth2/token",
                new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var obj = JsonSerializer.Deserialize<TwitterAuthToken>(responseContent);

            return obj.AccessToken;

        }

        public void StopStream()
        {
            _streamIsActive = false;
        }

        public TweetSummary GetSummary()
        {
            var retVal = new TweetSummary();
            retVal.TotalCount = _tweetCounter;
            retVal.TopTenHashtags = _hashTagsStore.OrderByDescending(e=>e.Count).Take(10).ToList();
            return retVal;
        }
    }
}
