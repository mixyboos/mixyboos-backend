using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MixyBoos.Api;

public class Program {
    private static readonly string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    private static readonly bool isDevelopment = environment == Environments.Development;

    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables();
        var configuration = builder.Build();

        return Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => {
                o.ValidateOnBuild = false;
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseKestrel(options => {
                        options.Listen(IPAddress.Any, 5001, listenOptions => // https
                        {
                            var pemFile = configuration["SSL:PemFile"];
                            var keyFile = configuration["SSL:KeyFile"];
                            if (string.IsNullOrEmpty(pemFile) || string.IsNullOrEmpty(keyFile)) {
                                return;
                            }

                            var certPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/fullchain.pem");
                            var keyPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/privkey.pem");
                            var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);

                            listenOptions.UseHttps(x509);
                        });
                    })
                    .UseStartup<Startup>();
            });
    }
}
