using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using MixyBoos.Api.Tests.Services;
using MixyBoos.Api.Tests.Services.AspNetCoreProtobuf.Tests;
using Moq;
using Xunit;

namespace MixyBoos.Api.Tests {
    public class TestAudioUpload : IClassFixture<WebApplicationFactory<Startup>> {
        const string ApiUrl = "https://dev.mixyboos.com:5001";
        private const string ClientId = "testharness";
        private const string ClientSecret = "e83ec86b-d234-4a09-bb91-6a36c43ccf77";
        private const string Scope = "openid api";
        private const string StsUrl = "https://dev.mixyboos.com:5001";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ApiTokenInMemoryClient _tokenService;

        public TestAudioUpload(WebApplicationFactory<Startup> factory) {
            _factory = factory;

            _tokenService = new ApiTokenInMemoryClient(
                StsUrl,
                new HttpClient(new HttpClientHandler {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                }));
        }

        private async Task SetTokenAsync(HttpClient client) {
            var accessToken = await _tokenService.GetApiToken(
                ClientId,
                Scope,
                ClientSecret
            );
            client.SetBearerToken(accessToken);
        }

        [Fact]
        public async Task Upload_AudioFile() {
            // Arrange
            var expectedContentType = "text/html; charset=utf-8";
            var url = $"{ApiUrl}/upload";
            var options = new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false,
            };
            var client = _factory.CreateDefaultClient(new InspectingDelegatingHandler());
            await SetTokenAsync(client);
            // Act
            HttpResponseMessage response;

            await using (var file1 = File.OpenRead(@"Fixtures/Audio/1-minute-sine.mp3"))
            using (var content1 = new StreamContent(file1))
            using (var formData = new MultipartFormDataContent()) {
                // Add file (file, field name, file name)
                formData.Add(content1, "files", "1-minute-sine.mp3");
                response = await client.PostAsync(url, formData);
            }

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(responseString);
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());

            response.Dispose();
            client.Dispose();
        }
    }
}
