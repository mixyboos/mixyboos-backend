using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

    [HttpGet("audiourl")]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MixDTO>> GetAudioUrl([FromQuery] string id) {
        var user = User?.Identity?.Name is not null ? await _userManager.FindByNameAsync(User.Identity.Name) : null;
        var mix = await _context.Mixes
            .Where(r => r.Id.Equals(Guid.Parse(id)))
            .SingleOrDefaultAsync();

        if (mix is null) {
            return NotFound();
        }

        //track this as a play
        _context.MixPlays.Add(new MixPlay {
            Mix = mix,
            User = user
        });
        await _context.SaveChangesAsync();
        return Ok(Flurl.Url.Combine(_config["LiveServices:ListenUrl"], mix.Id.ToString(), "manifest.mpd"));
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("feed")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MixDTO>>> GetFeed() {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var mixes = await _context.Mixes
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

    [HttpPatch]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MixDTO>> Patch([FromBody] MixDTO mix) {
        try {
            var entity = mix.Adapt<Mix>();
            var existing = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(mix.Id));
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

    [HttpDelete]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] string id) {
        try {
            var entity = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(id)));
            if (entity is null) {
                return NotFound();
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(StatusCodes.Status204NoContent);
        } catch (DbUpdateException ex) {
            _logger.LogError("Error creating mix {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
