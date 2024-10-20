using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Extensions;
using MixyBoos.Api.Services.Helpers;

namespace MixyBoos.Api.Controllers;

[Authorize]
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

  [HttpGet]
  public async Task<ActionResult<ProfileDTO>> GetProfile() {
    var user = await _userManager.FindByNameWithFollowingAsync(User.Identity.Name);
    if (user is null) {
      return Unauthorized();
    }

    return Ok(user.ToDto());
  }


  [HttpPost]
  [Authorize]
  [Consumes(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ProfileDTO>> UpdateProfile([FromBody] ProfileDTO incoming) {
    if (!ModelState.IsValid) {
      return BadRequest();
    }

    var user = await _userManager.FindByIdAsync(incoming.Id);
    if (user is null) {
      return NotFound();
    }

    user.FromDto(incoming);

    await _userManager.UpdateAsync(user);

    return Ok(user.ToDto());
  }

  [HttpPost("follow")]
  public async Task<IActionResult> FollowUser([FromQuery] string userId) {
    var userToFollow = await _userManager.FindByIdAsync(userId);
    var me = await _userManager.FindByNameWithFollowingAsync(User.Identity.Name);

    if (userToFollow is null && me is not null) {
      return BadRequest();
    }

    if (!me.Following.Any(f => f.Id.Equals(userToFollow.Id))) {
      me.Following.Add(userToFollow);
      userToFollow.Followers.Add(me);
      await _context.SaveChangesAsync();
    }

    return Ok();
  }

  [HttpPost("togglefollow")]
  public async Task<IActionResult> ToggleFollow([FromQuery] string slug) {
    var userToFollow = await _userManager.FindBySlugWithFollowingAsync(slug);
    var me = await _userManager.FindByNameWithFollowingAsync(User.Identity.Name);

    if (userToFollow is null && me is not null) {
      return BadRequest();
    }

    if (!me.Following.Any(f => f.Id.Equals(userToFollow.Id))) {
      me.Following.Add(userToFollow);
      userToFollow.Followers.Add(me);
    } else {
      me.Following.Remove(userToFollow);
      userToFollow.Followers.Remove(userToFollow);
    }

    await _context.SaveChangesAsync();
    return Ok();
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

  [HttpGet("{slug}")]
  public async Task<ActionResult<ProfileDTO>> GetBySlug(string slug) {
    var user = await _userManager.FindBySlugAsync(slug);
    if (user is null) {
      return NotFound();
    }

    return Ok(user.Adapt<ProfileDTO>());
  }
}
