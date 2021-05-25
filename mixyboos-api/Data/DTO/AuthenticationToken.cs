using System.Text.Json.Serialization;

namespace MixyBoos.Api.Data.DTO {
    public class AuthenticationToken {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }

        [JsonPropertyName("token_type")] public string TokenType { get; set; }

        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

        [JsonPropertyName("userName")] public string Username { get; set; }

        [JsonPropertyName(".issued")] public string IssuedAt { get; set; }

        [JsonPropertyName(".expires")] public string ExpiresAt { get; set; }
    }
}
