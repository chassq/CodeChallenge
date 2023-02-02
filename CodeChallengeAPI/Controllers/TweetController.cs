using CodeChallengeAPI.Models;
using CodeChallengeAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallengeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly ITwitterStreamService _twitterStreamService;

        public TweetController(ITwitterStreamService twitterStreamService)
        {
            _twitterStreamService = twitterStreamService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TweetSummary>> QueryTweetInfo()
        {
            var summary = _twitterStreamService.GetSummary();

            return await Task.FromResult(summary);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> StopTwitterVolumeStream()
        {
            _twitterStreamService.StopStream();

            return await Task.FromResult(new OkObjectResult($"Batch events created and batch processes commenced."));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> RestartTwitterVolumeStream()
        {
            await _twitterStreamService.StartStream();

            return await Task.FromResult(new OkObjectResult($"Batch events created and batch processes commenced."));
        }
    }
}