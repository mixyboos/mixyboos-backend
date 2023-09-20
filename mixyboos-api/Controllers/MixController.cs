using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Bogus;
using ExpressionDebugger;
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
using MixyBoos.Api.Services.Extensions;

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
    var script = mixes.BuildAdapter()
      .CreateMapExpression<MixDTO>()
      .ToScript();

    var result = mixes.Adapt<List<MixDTO>>();
    return Ok(result);
  }

  [HttpGet("user")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> GetByUser([FromQuery] string user) {
    var mixes = await _context.Mixes
      .Include(m => m.User)
      .Where(m => m.User.Slug.Equals(user))
      .Where(m => m.IsProcessed)
      .ToListAsync();
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
    if (mixes is null) {
      return NoContent();
    }

    var result = mixes.Adapt<MixDTO>();
    return Ok(result);
  }

  [HttpGet("audiourl")]
  [Produces(MediaTypeNames.Text.Plain)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<MixDTO>> GetAudioUrl([FromQuery] string id) {
    var mix = await _context.Mixes
      .Where(r => r.Id.Equals(Guid.Parse(id)))
      .SingleOrDefaultAsync();

    if (mix is null) {
      return NotFound();
    }

    //track this as a play
    //TODO: Re-enable this
    // await _context.MixPlays.AddAsync(new MixPlay() {
    //     Mix = mix,
    //     User = user
    // });
    // await _context.SaveChangesAsync();
    return Ok(Flurl.Url.Combine(_config["LiveServices:ListenUrl"], mix.Id.ToString(), $"{mix.Id}.m3u8"));
  }

  [Authorize]
  [HttpGet("feed")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> GetFeed() {
    var user = await _userManager.FindByNameAsync(User.Identity.Name);
    var mixes = await _context.Mixes
      .Where(m => m.User.Id.Equals(user.Id))
      .Where(m => m.IsProcessed)
      .OrderByDescending(m => m.DateCreated)
      .ToListAsync();
    var result = mixes.Adapt<List<MixDTO>>();
    return Ok(result);
  }

  [HttpPost]
  [Authorize]
  [Consumes(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<MixDTO>> Post([FromBody] MixDTO mix) {
    try {
      var entity = mix.Adapt<Mix>();
      var faker = new Faker();
      var user = await _userManager.FindByNameAsync(User.Identity.Name);
      entity.User = user;
      entity.Image = entity.Image ?? faker.Image.LoremFlickrUrl();

      //check if the file has been processed
      entity.IsProcessed = System.IO.File.Exists(
        Path.Combine(_config["AudioProcessing:OutputDir"], entity.Id.ToString(), $"{entity.Id}.m3u8"));

      await _context.AddOrUpdate<Mix>(entity);
      await _context.SaveChangesAsync();

      var response = entity.Adapt<MixDTO>();
      return CreatedAtAction(nameof(Get), new {id = response.Id}, response);
    } catch (DbUpdateException ex) {
      _logger.LogError("Error creating mix {Message}", ex.Message);
      return BadRequest(ex.Message);
    }
  }

  [HttpPatch]
  [Authorize]
  [Consumes(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<MixDTO>> Patch([FromBody] MixDTO mix) {
    try {
      var entity = mix.Adapt<Mix>();
      var existing = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(mix.Id)));
      if (existing is null) {
        return NotFound();
      }

      existing.IsProcessed = entity.IsProcessed;
      existing.Title = entity.Title;
      existing.Description = entity.Description;
      existing.Image = entity.Image;

      await _context.SaveChangesAsync();

      var response = existing.Adapt<MixDTO>();
      return CreatedAtAction(nameof(Get), new {id = response.Id}, response);
    } catch (DbUpdateException ex) {
      _logger.LogError("Error creating mix {Message}", ex.Message);
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("addlike")]
  [Authorize]
  [Consumes(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<MixDTO>> AddLike([FromBody] string id) {
    var user = await _userManager.FindByNameAsync(User.Identity.Name);
    if (user is null) {
      return BadRequest();
    }

    var mix = await _context
      .Mixes
      .FirstOrDefaultAsync(m => m.Id.Equals(id));
    if (mix is null) {
      return NotFound();
    }

    //
    // await _context.MixLikes.AddAsync(new MixLike {
    //     Mix = mix,
    //     User = user
    // });
    return Ok();
  }

  [HttpDelete]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete([FromQuery] string id) {
    try {
      var user = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(id)));
      if (user is null) {
        return NotFound();
      }

      var mix = await _context
        .Mixes
        .FirstOrDefaultAsync(m => m.Id.Equals(id));
      if (mix is null) {
        return NotFound();
      }

      _context.Remove(user);
      await _context.SaveChangesAsync();
      return Ok(StatusCodes.Status204NoContent);
    } catch (DbUpdateException ex) {
      _logger.LogError("Error creating mix {Message}", ex.Message);
      return BadRequest(ex.Message);
    }
  }
}
