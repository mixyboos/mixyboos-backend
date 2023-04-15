using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;

namespace MixyBoos.Api.Services.Startup;

public static class HttpClientBuilders {
    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => {
                Console.WriteLine($"Failed getting RTMP stream {msg.StatusCode} - {msg.ReasonPhrase}");
                return msg.StatusCode == System.Net.HttpStatusCode.NotFound;
            })
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration) {
        services.AddHttpClient("RTMP", httpClient => {
                httpClient.BaseAddress =
                    new Uri(configuration["LiveServices:RtmpUrl"] ?? "https://live.mixyboos.com");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Set lifetime to five minutes
            .AddPolicyHandler(GetRetryPolicy());
    }
}
