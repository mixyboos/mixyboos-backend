using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
    var googleClientId = config["Auth:Google:ClientId"];
    var googleClientSecret = config["Auth:Google:ClientSecret"];
    if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret)) {
      services.AddAuthentication(options => {
          options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie(options => {
          options.Cookie.Name = ".MixyBoos.Cookies";
          options.Cookie.SameSite = SameSiteMode.None;
          options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddGoogle(options => {
          options.ClientId = googleClientId;
          options.ClientSecret = googleClientSecret;
          options.SignInScheme = IdentityConstants.ExternalScheme;
          options.CorrelationCookie.Name = config["Auth:CorrelationCookieName"];
          options.CorrelationCookie.SameSite = SameSiteMode.None;
          options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
          options.Scope.Add("profile");
          options.ClaimActions.MapJsonKey("picture", "picture", "url"); // Map the picture claim
        });
    }

    services.AddAuthorization();
    services.ConfigureApplicationCookie(options => {
      options.Cookie.Name = config["Auth:AuthCookieName"];
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
