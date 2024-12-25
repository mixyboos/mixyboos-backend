using System;
using System.Collections.Generic;
using System.Linq;
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
using MixyBoos.Api.Data.Repositories;
using MixyBoos.Api.Services.Extensions;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class MixController(
  MixyBoosContext __context,
  MixRepository repository,
  IRepository<MixLike> mixLikeRepository,
  IConfiguration config,
  UserManager<MixyBoosUser> userManager,
  ILogger<MixController> logger)
  : _Controller(logger) {
  [HttpGet]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> Get() {
    var mixes = await __context.Mixes.Include(m => m.User).ToListAsync();

    var script = mixes.BuildAdapter()
      .CreateMapExpression<MixDTO>()
      .ToScript();

    var result = mixes.Adapt<List<MixDTO>>();
    return Ok(result);
  }

  [HttpGet("me")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> GetMyMixes() {
    var user = await userManager.FindByNameAsync(User.Identity.Name);
    if (user is null) {
      return Unauthorized();
    }

    var mixes = await repository.GetByUser(user.Slug);
    return Ok(mixes.Adapt<List<MixDTO>>());
  }

  [HttpGet("user")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> GetByUser([FromQuery] string user) {
    var mixes = await repository.GetByUser(user);
    return Ok(mixes.Adapt<List<MixDTO>>());
  }

  [HttpGet("single")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<MixDTO>> GetByUserAndSlug([FromQuery] string user, [FromQuery] string mix) {
    var result = await repository.GetByUserAndSlug(user, mix);
    if (result is null) {
      return NoContent();
    }

    return Ok(result.Adapt<MixDTO>());
  }

  [HttpGet("audiourl")]
  [Produces(MediaTypeNames.Text.Plain)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<MixDTO>> GetAudioUrl([FromQuery] Guid id) {
    var mix = await repository.Get(id);

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
    return Ok(
      Flurl.Url.Combine(
        config["LiveServices:ListenUrl"],
        mix.Id.ToString(),
        $"{mix.Id}.m3u8"));
  }

  [Authorize]
  [HttpGet("feed")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<List<MixDTO>>> GetFeed() {
    var user = await userManager.FindByNameAsync(User.Identity.Name);
    var mixes = await repository.GetFeedForUser(user.Id);
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
      var existing = await __context.Mixes
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id.Equals(mix.Id));
      if (existing is not null) {
        //we have a proxy mix from waveform generation
        //that completed before the form was submitted
        entity.IsProcessed = existing.IsProcessed;
        entity.Duration = existing.Duration;
      }

      var faker = new Faker();
      var user = await userManager.FindByNameAsync(User.Identity.Name);
      entity.User = user;
      entity.Image ??= faker.Image.LoremFlickrUrl();
      await repository.AddOrUpdate(entity);

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
      var existing = await repository.Get(mix.Id);

      if (existing is null) {
        return NotFound();
      }

      existing.IsProcessed = entity.IsProcessed;
      existing.Title = entity.Title;
      existing.Description = entity.Description;
      existing.Image = entity.Image;

      await repository.Update(existing);

      var response = existing.Adapt<MixDTO>();
      return CreatedAtAction(nameof(Get), new {id = response.Id}, response);
    } catch (DbUpdateException ex) {
      _logger.LogError("Error creating mix {Message}", ex.Message);
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("togglelike")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<MixDTO>> ToggleLike(Guid id) {
    if (id.Equals(Guid.Empty)) {
      return BadRequest();
    }

    var user = await userManager.FindByNameAsync(User.Identity.Name);
    if (user is null) {
      return Unauthorized();
    }

    var likes = await mixLikeRepository
      .GetAll()
      .Where(l => l.MixId.Equals(id) && l.UserId.Equals(user.Id))
      .ToListAsync();

    var mix = await repository.Get(id);
    if (mix is null) {
      return NotFound();
    }

    if (likes.Count != 0) {
      __context.RemoveRange(likes);
      await __context.SaveChangesAsync();
      return Ok(mix.Adapt<MixDTO>());
    }

    await __context.MixLikes.AddAsync(new MixLike {
      Mix = mix,
      User = user
    });
    await __context.SaveChangesAsync();
    return Ok(mix.Adapt<MixDTO>());
  }

  [HttpDelete]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete([FromQuery] Guid id) {
    try {
      var mix = await repository.Get(id);
      if (mix is null) {
        return NotFound();
      }

      await repository.Delete(mix);
      return Ok(StatusCodes.Status204NoContent);
    } catch (DbUpdateException ex) {
      _logger.LogError("Error creating mix {Message}", ex.Message);
      return BadRequest(ex.Message);
    }
  }
}
