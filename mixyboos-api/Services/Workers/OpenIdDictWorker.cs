using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MixyBoos.Api.Services.Workers {
  public class OpenIdDictWorker : IHostedService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OpenIdDictWorker> _logger;

    public OpenIdDictWorker(IServiceProvider serviceProvider, ILogger<OpenIdDictWorker> logger) {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
      using var scope = _serviceProvider.CreateScope();

      var manager = scope
        .ServiceProvider
        .GetRequiredService<IOpenIddictApplicationManager>();
      try {
        _logger.LogInformation("Creating the webclient OpenIddict application");
        if (await manager.FindByClientIdAsync("webclient", cancellationToken) is null) {
          _logger.LogInformation("App webclient does not exist, creating");
          await manager.CreateAsync(new OpenIddictApplicationDescriptor {
            ClientId = "webclient",
            DisplayName = "MixyBoos Web Client",
            ConsentType = ConsentTypes.Implicit,
            Permissions = {
              Permissions.Endpoints.Authorization,
              Permissions.Endpoints.Logout,
              Permissions.Endpoints.Token,
              Permissions.GrantTypes.Password,
              Permissions.GrantTypes.RefreshToken,
              "urn:ietf:params:oauth:grant-type:google_identity_token",
              Permissions.Prefixes.Scope + "api"
            }
          }, cancellationToken);
          _logger.LogInformation("App webclient created");
        }

        if (await manager.FindByClientIdAsync("testharness", cancellationToken) is null) {
          await manager.CreateAsync(new OpenIddictApplicationDescriptor {
            ClientId = "testharness",
            DisplayName = "Test Harness",
            ConsentType = ConsentTypes.Explicit,
            Permissions = {
              Permissions.Endpoints.Authorization,
              Permissions.Endpoints.Logout,
              Permissions.Endpoints.Token,
              Permissions.GrantTypes.Password,
              Permissions.GrantTypes.RefreshToken,
              Permissions.Prefixes.Scope + "api"
            }
          }, cancellationToken);
        }
      } catch (Npgsql.PostgresException e) {
        _logger.LogInformation("Error creating openiddict app {Error}", e.Message);
        //most likely the db hasn't been created yet
      }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
