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

            var context = scope.ServiceProvider.GetRequiredService<MixyBoosContext>();
            await context.Database.EnsureCreatedAsync();

            var manager =
                scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("console") is null) {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "console",
                    ClientSecret = "f30ce517-4219-46da-9cac-46a93d2cd57e",
                    DisplayName = "MixyBoos Client",
                    Permissions = {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials
                    }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
