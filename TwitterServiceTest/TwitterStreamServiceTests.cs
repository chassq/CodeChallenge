using Microsoft.Extensions.DependencyInjection;
using TwitterService.Service;

namespace TwitterServiceTest
{
    public class TwitterStreamServiceTests : BaseTest
    {
        private readonly ITwitterStreamService _twitterStreamService;

        public TwitterStreamServiceTests()
        {
            _twitterStreamService = ConfigFixture.Services.GetRequiredService<ITwitterStreamService>();
        }

        [Fact]
        public async Task EnsureStreamServiceSuccess()
        {
            await _twitterStreamService.StartStream();
            await Task.Delay(2000); //Delay on purpose to allow time for the service to start running.
            Assert.True(_twitterStreamService.IsStreamActive);

            var summary1 = _twitterStreamService.GetSummary();
            if (summary1 == null)
            {
                Assert.Fail("Stream summary1 was null.");
            }

            await Task.Delay(500); //Delay on purpose in order to get some tweets and increase counts.

            var summary2 = _twitterStreamService.GetSummary();
            if (summary2 == null)
            {
                Assert.Fail("Stream summary2 was null.");
            }

            Assert.True(summary2.TotalCount > summary1.TotalCount);

            _twitterStreamService.StopStream();

            Assert.False(_twitterStreamService.IsStreamActive);

        }
    }
}