using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Helpers;
using OpenIddict.Abstractions;

namespace MixyBoos.Api.Services.Auth {
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<MixyBoosUser, IdentityRole<Guid>> {
        private readonly ImageHelper _imageHelper;

        public ClaimsPrincipalFactory(
            UserManager<MixyBoosUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            ImageHelper imageHelper)
            : base(userManager, roleManager, optionsAccessor) {
            _imageHelper = imageHelper;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(MixyBoosUser user) {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaims(new[] {
                new Claim(OpenIddictConstants.Claims.Name, user.UserName ?? string.Empty),
                new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
                new Claim("displayName", user.DisplayName),
                new Claim("profileImage", _imageHelper.GetSmallImageUrl("users/avatars", user.ProfileImage)),
                new Claim("slug", user.Slug)
            });
            return identity;
        }
    }
}
