using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Startup.Mapster;

namespace MixyBoos.Api.Services.Startup;

public static class AuthenticationStartup {
  public static IServiceCollection AddMixyboosAuthentication(this IServiceCollection services, IConfiguration config) {
    services.AddAuthorization();
    services.ConfigureApplicationCookie(options => {
      options.Cookie.Name = config["Auth:CookieName"];
      options.Cookie.Domain = config["Auth:DomainName"];
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      options.Cookie.HttpOnly = true;
    });
    services
      .AddIdentityApiEndpoints<MixyBoosUser>()
      .AddEntityFrameworkStores<MixyBoosContext>();

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
    services.RegisterHttpClients(config);

    return services;
  }
}
