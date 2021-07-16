using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace MixyBoos.Api.Services.Auth {
    public class ClaimsTransformer : IClaimsTransformation {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) {
            //Add custom claim  - I could not access the database
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim("TenantId", "0000000000000001"));

            return Task.FromResult(principal);
        }
    }
}
