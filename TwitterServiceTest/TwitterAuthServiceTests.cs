using Microsoft.Extensions.DependencyInjection;
using TwitterService.Service;

namespace TwitterServiceTest
{
    public class TwitterAuthServiceTests : BaseTest
    {
        private readonly ITwitterAuthService _twitterAuthService;

        public TwitterAuthServiceTests()
        {
            _twitterAuthService = ConfigFixture.Services.GetRequiredService<ITwitterAuthService>();
        }

        [Fact]
        public async Task EnsureGetBearerTokenSuccess()
        { 
            var bt = await _twitterAuthService.GetBearerToken();
            Assert.True(!string.IsNullOrWhiteSpace(bt));
        }
    }
}