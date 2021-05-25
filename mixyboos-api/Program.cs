using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MixyBoos.Api {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseKestrel(options => {
                            options.Listen(IPAddress.Any, 5000); // http
                            options.Listen(IPAddress.Any, 5001, listenOptions => // https
                            {
                                listenOptions.UseHttps(
                                    "/home/fergalm/dev/mixyboos/certs/dev.mixyboos.com.pfx", 
                                    "secret");
                            });
                        })
                        .UseStartup<Startup>();
                });
    }
}
