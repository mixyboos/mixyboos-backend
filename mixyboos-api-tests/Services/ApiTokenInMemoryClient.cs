using IdentityModel.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MixyBoos.Api.Data.DTO;

namespace MixyBoos.Api.Tests.Services {
    namespace AspNetCoreProtobuf.Tests {
        public class ApiTokenInMemoryClient {
            private readonly HttpClient _httpClient;
            private readonly string _stsServerUrl;

            private class AccessTokenItem {
                public string AccessToken { get; set; } = string.Empty;
                public DateTime ExpiresIn { get; set; }
            }

            private ConcurrentDictionary<string, AccessTokenItem> _accessTokens = new();

            public ApiTokenInMemoryClient(
                string stsServerUrl,
                HttpClient httpClient) {
                _httpClient = httpClient;
                _stsServerUrl = stsServerUrl;
            }

            public async Task<string> GetApiToken(string clientId, string scope, string clientSecret) {
                if (_accessTokens.ContainsKey(clientId)) {
                    var accessToken = _accessTokens.GetValueOrDefault(clientId);
                    if (accessToken.ExpiresIn > DateTime.UtcNow) {
                        return accessToken.AccessToken;
                    } else {
                        // remove
                        _accessTokens.TryRemove(clientId, out AccessTokenItem accessTokenItem);
                    }
                }

                var newAccessToken = await getApiToken(clientId, scope, clientSecret);
                _accessTokens.TryAdd(clientId, newAccessToken);

                return newAccessToken.AccessToken;
            }

            private async Task<AccessTokenItem> getApiToken(string clientId, string scope, string clientSecret) {
                try {
                    var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(
                        _httpClient,
                        _stsServerUrl);

                    if (disco.IsError) {
                        throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
                    }

                    var request = new HttpRequestMessage(HttpMethod.Post, disco.TokenEndpoint) {
                        Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                            new("username", "fergal.moran+mixyboos@gmail.com"),
                            new("password", "SVqVKJWZh5dIaM7JsNY1h0E/xbzPCD7y7Veedxa1Q/k="),
                            new("scope", scope),
                            new("client_id", clientId),
                            new("grant_type", "password")
                        })
                    };

                    var tokenResponse = await _httpClient.SendAsync(request);
                    if (!tokenResponse.IsSuccessStatusCode) {
                        throw new ApplicationException(
                            $"Status code: {!tokenResponse.IsSuccessStatusCode}, Error: {tokenResponse.ReasonPhrase}");
                    }

                    var content = await tokenResponse.Content.ReadAsStringAsync();
                    var accessToken = JsonSerializer.Deserialize<AuthenticationToken>(content);


                    return new AccessTokenItem {
                        ExpiresIn = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn),
                        AccessToken = accessToken.AccessToken
                    };
                } catch (Exception e) {
                    throw new ApplicationException($"Exception {e}");
                }
            }
        }
    }
}
