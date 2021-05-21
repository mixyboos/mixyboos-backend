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

            // await manager.DeleteAsync("webclient");
            if (await manager.FindByClientIdAsync("webclient") is null) {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "webclient",
                    DisplayName = "MixyBoos Web Client",
                    ConsentType = ConsentTypes.Implicit,
                    PostLogoutRedirectUris = {
                        new Uri("https://localhost:5002/oidc-signout")
                    },
                    RedirectUris = {
                        new Uri("https://localhost:5002/oidc-signin"),
                        new Uri("https://localhost:5002/oidc-silent-refresh")
                    },
                    Permissions = {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.Password,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Prefixes.Scope + "api"
                    }
                });
            }

            if (await manager.FindByClientIdAsync("postman", cancellationToken) is null) {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "postman",
                    ClientSecret = "postman-secret",
                    DisplayName = "Postman",
                    RedirectUris = {new Uri("https://oauth.pstmn.io/v1/callback")},
                    Permissions = {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Permissions.ResponseTypes.Code
                    }
                }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
