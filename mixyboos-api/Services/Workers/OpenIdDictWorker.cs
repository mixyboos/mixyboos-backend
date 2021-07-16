using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MixyBoos.Api.Services.Workers {
    public class OpenIdDictWorker : IHostedService {
        private readonly IServiceProvider _serviceProvider;

        public OpenIdDictWorker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken) {
            using var scope = _serviceProvider.CreateScope();

            var manager = scope
                .ServiceProvider
                .GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("webclient", cancellationToken) is null) {
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
                        Permissions.Prefixes.Scope + "api"
                    }
                }, cancellationToken);
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
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
