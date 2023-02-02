using System.Text.Json.Serialization;

namespace TwitterService.Model
{
    public class TwitterAuthToken
    {
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}
