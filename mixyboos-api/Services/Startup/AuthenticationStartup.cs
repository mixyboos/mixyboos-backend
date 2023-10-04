using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Startup.Mapster;

namespace MixyBoos.Api.Services.Startup;

public static class AuthenticationStartup {
  public static IServiceCollection AddMixyboosAuthentication(this IServiceCollection services, IConfiguration config) {
    services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
    services.AddAuthorizationBuilder();

    services
      .AddIdentityCore<MixyBoosUser>()
      .AddEntityFrameworkStores<MixyBoosContext>()
      .AddApiEndpoints();

    services.Configure<IdentityOptions>(options => {
      // Default Password settings.
      options.SignIn.RequireConfirmedEmail = false;
      options.User.RequireUniqueEmail = true;
      options.Password.RequireDigit = false;
      options.Password.RequireLowercase = false;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequiredLength = 4;
      options.Password.RequiredUniqueChars = 0;
    });
    services.RegisterMapsterConfiguration(config);
    services.RegisterHttpClients(config);

    return services;
  }
}
