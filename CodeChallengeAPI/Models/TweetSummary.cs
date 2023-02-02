﻿namespace CodeChallengeAPI.Models
{
    public class TweetSummary
    {
        public long TotalCount { get; set; }
        public List<TweetHashtag> TopTenHashtags { get; set; } = new List<TweetHashtag>();
    }
}
