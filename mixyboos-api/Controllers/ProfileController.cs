using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Services.Helpers;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class ProfileController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly MixyBoosContext _context;

        public ProfileController(
            UserManager<MixyBoosUser> userManager,
            MixyBoosContext context,
            ILogger<ProfileController> logger) : base(logger) {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ProfileDTO>> GetProfile() {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(user.Adapt<ProfileDTO>());

            // return Ok(new ProfileDTO {
            //     DisplayName = user.UserName
            // });
        }

        [HttpGet("apikey")]
        public async Task<ActionResult<GetApiKeyDTO>> GetApiKey() {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (string.IsNullOrEmpty(user.StreamKey)) {
                user.StreamKey = KeyGenerator.GenerateRandomCryptoString(32);
                await _context.SaveChangesAsync();
            }

            return Ok(new GetApiKeyDTO {
                UserId = User.Identity.Name,
                ApiKey = user.StreamKey
            });
        }
    }
}
