using System.Text.Json.Serialization;

namespace CodeChallengeAPI
{
    public class Tweet
    {
        [JsonPropertyName("data")]
        public TweetData? TweetData { get; set; }
    }

    public class TweetData
    {
        [JsonPropertyName("author_id")]
        public string? AuthorId { get; set; }

        [JsonPropertyName("entities")]
        public TweetEntity? TweetEntity { get; set; }
    }

    public class TweetEntity
    {
        [JsonPropertyName("hashtags")]
        public List<TweetHashtag>? TweetHashTags { get; set; }
    }

    public class TweetHashtag
    {
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }

        public long Count { get; set; }
    }
}