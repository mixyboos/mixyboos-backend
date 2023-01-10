using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Bogus;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class MixController : _Controller {
    private readonly MixyBoosContext _context;
    private readonly IConfiguration _config;
    private readonly UserManager<MixyBoosUser> _userManager;

    public MixController(MixyBoosContext context,
        IConfiguration config,
        UserManager<MixyBoosUser> userManager,
        ILogger<MixController> logger) : base(logger) {
        _context = context;
        _config = config;
        _userManager = userManager;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MixDTO>>> Get() {
        var mixes = await _context.Mixes.Include(m => m.User).ToListAsync();
        var result = mixes.Adapt<List<MixDTO>>();
        return Ok(result);
    }

    [HttpGet("single")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MixDTO>> GetByUserAndMix([FromQuery] string user, [FromQuery] string mix) {
        var mixes = await _context.Mixes
            .Where(m => m.User.Slug.Equals(user))
            .Where(m => m.Slug.Equals(mix))
            .Include(m => m.User).FirstOrDefaultAsync();
        var result = mixes.Adapt<MixDTO>();
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("feed")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MixDTO>>> GetFeed() {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var mixes = await _context.Mixes.Include(m => m.User)
            .Include(m => m.User.Followers)
            .Include(m => m.User.Following)
            .Where(m => m.User.Id.Equals(user.Id))
            .OrderByDescending(m => m.DateCreated)
            .ToListAsync();
        var result = mixes.Adapt<List<MixDTO>>();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MixDTO>> Post([FromBody] MixDTO mix) {
        try {
            var entity = mix.Adapt<Mix>();
            var faker = new Faker();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            entity.User = user;
            entity.Image = faker.Image.LoremFlickrUrl();

            //check if the file has been processed
            entity.IsProcessed = System.IO.File.Exists(
                Path.Combine(_config["AudioProcessing:OutputDir"], entity.Id.ToString(), "manifest.mpd"));

            await _context.Mixes.AddAsync(entity);
            await _context.SaveChangesAsync();

            var response = entity.Adapt<MixDTO>();
            return CreatedAtAction(nameof(Get), new {id = response.Id}, response);
        } catch (DbUpdateException ex) {
            _logger.LogError("Error creating mix {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
