using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MixyBoos.Api.Services.Startup;

public static class LoggingStartup {
  public static WebApplicationBuilder CreateLogger(
    this WebApplicationBuilder builder,
    IConfiguration config) {
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((hostContext, services, configuration) => {
      configuration.ReadFrom.Configuration(config);
      configuration.WriteTo.Console();
    });
    return builder;
  }
}
