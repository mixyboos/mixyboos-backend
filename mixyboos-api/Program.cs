using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MixyBoos.Api;

public class Program {
    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        var certPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/fullchain.pem");
        var keyPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/privkey.pem");
        var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);

        return Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => {
                o.ValidateOnBuild = false;
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseKestrel(options => {
                        options.Listen(IPAddress.Any, 5001, listenOptions => // https
                        {
                            listenOptions.UseHttps(x509);
                        });
                    })
                    .UseStartup<Startup>();
            });
    }
}
