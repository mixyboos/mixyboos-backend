using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace MixyBoos.Api;

public class Program {
    private static readonly string _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public static void Main(string[] args) {
        var logger = LogManager.Setup()
            .LoadConfigurationFromAppSettings()
            .GetCurrentClassLogger();

        try {
            logger.Debug("init main");
            CreateHostBuilder(args).Build().Run();
        } catch (Exception exception) {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        } finally {
            NLog.LogManager.Shutdown();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{_environment}.json", optional: true)
            .AddEnvironmentVariables();


        var configuration = builder.Build();

        return Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => {
                o.ValidateOnBuild = false;
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseKestrel(options => {
                        var pemFile = configuration["SSL:PemFile"];
                        var keyFile = configuration["SSL:KeyFile"];
                        if (string.IsNullOrEmpty(pemFile) || string.IsNullOrEmpty(keyFile)) {
                            return;
                        }

                        options.Listen(IPAddress.Any, 5001, listenOptions => {
                            var certPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/fullchain.pem");
                            var keyPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/privkey.pem");
                            var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);

                            listenOptions.UseHttps(x509);
                        });
                    })
                    .UseStartup<Startup>()
                    .ConfigureLogging(logging => {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    })
                    .UseNLog();
            });
    }
}
