using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class AuthController(
  SignInManager<MixyBoosUser> signInManager,
  UserManager<MixyBoosUser> userManager,
  IConfiguration config) : ControllerBase {
  [HttpGet("google-login")]
  public IActionResult GoogleLogin() {
    var properties = new AuthenticationProperties {
      RedirectUri = $"{config["SiteSettings:ApiUrl"]}/auth/google-callback",
      Items = {
        {"returnUrl", config["SiteSettings:WebUrl"]}
      }
    };
    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
  }

  [HttpGet("google-callback")]
  [HttpGet("/signin-google")]
  public async Task<IActionResult> GoogleCallback() {
    var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

    if (!authenticateResult.Succeeded)
      return Unauthorized();

    var claims = authenticateResult.Principal.Claims;
    foreach (var claim in claims) {
      Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
    }

    var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
    var imageUrl = authenticateResult.Principal.FindFirstValue("picture") ??
                   authenticateResult.Principal.FindFirstValue(ClaimTypes.Uri);

    var user = await userManager.FindByEmailAsync(email);

    if (user == null) {
      // Create the user if they don't exist
      user = new MixyBoosUser {
        UserName = email,
        Email = email,
        EmailConfirmed = true, // Google has already verified the email
        ProfileImage = imageUrl
      };
      var result = await userManager.CreateAsync(user);
      if (!result.Succeeded)
        return BadRequest(result.Errors);
    }

    // Sign in the user with Identity cookie
    await signInManager.SignInAsync(user, isPersistent: true);

    // Redirect back to the React app
    var frontendUrl = config["SiteSettings:WebUrl"];
    return Redirect(frontendUrl ?? "/");
  }
}
