using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwitterService.Config;
using TwitterService.Model;

namespace TwitterService.Service
{
    public interface ITwitterStreamService
    {
        bool IsStreamActive { get; }
        Task StartStream();
        void StopStream();
        TweetSummary GetSummary();
    }

    public class TwitterStreamService : ITwitterStreamService
    {
        private readonly IServiceProvider _services;

        protected readonly IConfiguration _configuration;

        private readonly TwitterConfig _twitterConfig;

        public bool IsStreamActive 
        { 
            get { return _streamIsActive; } 
        }

        private bool _streamIsActive = false;

        private long _tweetCounter = 0;

        private readonly List<TweetHashtag> _hashTagsStore = new();

        public TwitterStreamService(IServiceProvider services
            , IConfiguration configuration)
        {
            _services = services;

            _configuration = configuration;

            _twitterConfig = _configuration.GetSection(nameof(TwitterConfig))?.Get<TwitterConfig>()
                                ?? throw new ArgumentNullException("The twitter configuration is not set properly. Please see the readme.md file at the root of this project for instructions.");
        }

        /// <summary>
        /// Method will start the streaming service and begin receiving tweets. This method will put the stream listener
        /// on a worker thread and return after a small delay.
        /// </summary>
        /// <returns></returns>
        public async Task StartStream()
        {
            //Run the streaming on a worker thread in order to not block other service requests.
            _ = Task.Run(async () =>
            {
                using var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope();

                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ITwitterStreamService>>();

                var authService = scope.ServiceProvider.GetRequiredService<ITwitterAuthService>();

                try {

                    // Create a new HttpClient to call the Twitter volume stream API
                    var httpClient = new HttpClient();

                    httpClient.DefaultRequestHeaders.Add("User-Agent", authService.TwitterApplicationName);

                    //TODO - Investigate parameterization of the rules query string
                    var url = $"{_twitterConfig.VolumeStreamEndpoint}?tweet.fields=entities&expansions=author_id&user.fields=created_at";

                    var bearerToken = await authService.GetBearerToken();

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();

                    _streamIsActive = true;

                    using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, true); 

                    while (_streamIsActive)
                    {
                        //TODO - Investigate rate limiting behaviors in order to handle 429 error codes based on the application request rate limits. 
                        //See https://developer.twitter.com/en/docs/authentication/oauth-2-0/application-only
                        var line = await reader.ReadLineAsync();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var tweet = JsonSerializer.Deserialize<Tweet>(line);

                            if(tweet != null) _tweetCounter++; //If a tweet exists then increment the Tweet count

                            //Verify is the tweet contains hashtags and if so process them.
                            var hashtags = tweet?.TweetData?.TweetEntity?.TweetHashTags;
                            if (hashtags != null && hashtags.Any())
                            {    
                                foreach( var hashtag in hashtags)
                                {
                                    //If a hashtag already exists in the hashtag store then increment the instance count of the existing hashtag.
                                    //Otherwise add the new hashtag to the hashtag store with a count of 1.
                                    if (_hashTagsStore.Any(t => !string.IsNullOrWhiteSpace(t.Tag) 
                                                                && t.Tag.Equals(hashtag.Tag, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        //Existing hashtag handling
                                        var updateTag = _hashTagsStore.Where(t => !string.IsNullOrWhiteSpace(t.Tag) 
                                                                                    && t.Tag.Equals(hashtag.Tag, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                                        if(updateTag != null)
                                        {
                                            updateTag.Count++;
                                        }
                                    }
                                    else
                                    {
                                        //New hashtag handling
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

        /// <summary>
        /// Method will set the streaming service to be in an inactive state which will stop the streaming.
        /// </summary>
        public void StopStream()
        {
            _streamIsActive = false;
        }

        /// <summary>
        /// Method returns a current summary of the streaming data.
        /// </summary>
        public TweetSummary GetSummary()
        {
            return new TweetSummary
            {
                TotalCount = _tweetCounter,
                TopTenHashtags = _hashTagsStore.OrderByDescending(e => e.Count).Take(10).ToList()
            };
        }
    }
}
