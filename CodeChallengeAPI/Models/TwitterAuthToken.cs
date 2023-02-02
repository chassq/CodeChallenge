using System.Text.Json.Serialization;

namespace CodeChallengeAPI.Models
{
    public class TwitterAuthToken
    {
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}
