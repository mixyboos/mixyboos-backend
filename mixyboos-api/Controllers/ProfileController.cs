using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class ProfileController : _Controller {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger) : base(logger) {
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ProfileDTO>> GetProfile() {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(new ProfileDTO {
                DisplayName = user.UserName
            });
        }
    }
}
