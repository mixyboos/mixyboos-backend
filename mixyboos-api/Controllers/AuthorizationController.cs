using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MixyBoos.Api.Data;
using MixyBoos.Api.Services.Helpers;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace MixyBoos.Api.Controllers {
    public class AuthorizationController : _Controller {
        const int ACCESS_TOKEN_EXPIRY = 43800; //minutes
        const int REFRESH_TOKEN_EXPIRY = 30; //days

        private readonly SignInManager<MixyBoosUser> _signInManager;
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly ImageHelper _imageHelper;

        public AuthorizationController(
            SignInManager<MixyBoosUser> signInManager,
            UserManager<MixyBoosUser> userManager,
            ImageHelper imageHelper,
            ILogger<AuthorizationController> logger) : base(logger) {
            _signInManager = signInManager;
            _userManager = userManager;
            _imageHelper = imageHelper;
        }


        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange() {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request.IsClientCredentialsGrantType() || request.IsPasswordGrantType()) {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null) {
                    var properties = new AuthenticationProperties(new Dictionary<string, string> {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                if (!await _signInManager.CanSignInAsync(user) ||
                    (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))) {
                    var properties = new AuthenticationProperties(new Dictionary<string, string> {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "This user is currently not allowed to sign in."
                    });
                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                var result =
                    await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded) {
                    var properties = new AuthenticationProperties(new Dictionary<string, string> {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    });
                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                if (_userManager.SupportsUserLockout) {
                    await _userManager.ResetAccessFailedCountAsync(user);
                }

                //TODO: This probably shouldn't be here

                // Create a new ClaimsPrincipal containing the claims that
                // will be used to create an id_token, a token or a code.
                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                principal.SetScopes(new[] {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.OfflineAccess,
                    Scopes.Roles
                }.Intersect(request.GetScopes()));

                principal.SetAccessTokenLifetime(TimeSpan.FromMinutes(ACCESS_TOKEN_EXPIRY))
                    .SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(ACCESS_TOKEN_EXPIRY))
                    .SetIdentityTokenLifetime(TimeSpan.FromMinutes(ACCESS_TOKEN_EXPIRY))
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(REFRESH_TOKEN_EXPIRY));

                foreach (var claim in principal.Claims) {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsRefreshTokenGrantType()) {
                // Retrieve the claims principal stored in^ the refresh token.
                var info = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Retrieve the user profile corresponding to the refresh token.
                // Note: if you want to automatically invalidate the refresh token
                // when the user password/roles change, use the following line instead:
                // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
                var user = await _userManager.GetUserAsync(info.Principal);
                if (user == null) {
                    var properties = new AuthenticationProperties(new Dictionary<string, string> {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The refresh token is no longer valid."
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                // Ensure the user is still allowed to sign in.
                if (!await _signInManager.CanSignInAsync(user)) {
                    var properties = new AuthenticationProperties(new Dictionary<string, string> {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is no longer allowed to sign in."
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                // Create a new ClaimsPrincipal containing the claims that
                // will be used to create an id_token, a token or a code.
                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                foreach (var claim in principal.Claims) {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not implemented.");
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize() {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // If the user principal can't be extracted, redirect the user to the login page.
            if (!result.Succeeded) {
                return Challenge(
                    authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            // Create a new claims principal
            var claims = new List<Claim> {
                // 'subject' claim which is required
                new Claim(OpenIddictConstants.Claims.Subject, result.Principal.Identity.Name),
                new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set requested scopes (this is not done automatically)
            claimsPrincipal.SetScopes(request.GetScopes());

            // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal) {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type) {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
