using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MixyBoos.Api.Data;
using OpenIddict.Abstractions;

namespace MixyBoos.Api.Services.Auth {
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<MixyBoosUser, IdentityRole> {
        public ClaimsPrincipalFactory(
            UserManager<MixyBoosUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(MixyBoosUser user) {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaims(new[] {
                new Claim(OpenIddictConstants.Claims.Name, user.UserName ?? string.Empty),
                new Claim("displayName", user.DisplayName),
                new Claim("profileImage", user.ProfileImage),
                new Claim("slug", user.Slug)
            });
            return identity;
        }
    }
}
