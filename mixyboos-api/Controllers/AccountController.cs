using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;

namespace MixyBoos.Api.Controllers {
    [Authorize]
    [Route("[controller]")]
    public class AccountController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly MixyBoosContext _context;

        public AccountController(
            UserManager<MixyBoosUser> userManager,
            MixyBoosContext dbContext,
            ILogger<AccountController> logger) : base(logger) {
            _userManager = userManager;
            _context = dbContext;
        }

        //
        // POST: /Account/Register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var faker = new Faker("en");
            var user = new MixyBoosUser {
                UserName = model.UserName,
                Email = model.UserName,
                DisplayName = model.DisplayName,
                Image = faker.Internet.Avatar()
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                return Ok(model);
            }

            AddErrors(result);

            // If we got this far, something failed.
            return BadRequest(ModelState);
        }

        #region Helpers

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
