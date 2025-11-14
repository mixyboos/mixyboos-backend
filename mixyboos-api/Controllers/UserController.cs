using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class UserController : _Controller {
  private readonly MixyBoosContext _context;

  public UserController(MixyBoosContext context, UserManager<MixyBoosUser> userManager, ILogger<UserController> logger)
    : base(userManager,
      logger) {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<UserDTO>> GetUserBySlug([FromQuery] string slug) {
    var user = await _context.Users
      .Include(u => u.Followers)
      .Include(u => u.Following)
      .FirstOrDefaultAsync(u => u.Slug == slug);
    if (user is not null) {
      return Ok(user.Adapt<UserDTO>());
    }

    return NotFound();
  }
}
