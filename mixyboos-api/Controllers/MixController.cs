using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Route("[controller]")]
    public class MixController : _Controller {
        private readonly MixyBoosContext _context;
        private readonly UserManager<MixyBoosUser> _userManager;

        public MixController(MixyBoosContext context,
            UserManager<MixyBoosUser> userManager,
            ILogger<MixController> logger) : base(logger) {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MixDTO>>> Get() {
            var mixes = await _context.Mixes.Include(m => m.User).ToListAsync();
            TypeAdapterConfig<Mix, MixDTO>
                .NewConfig()
                .Map(dest => dest.AudioUrl, src => $"http://localhost:8080/hls/{src.Id}/pl.m3u8")
                .Map(dest => dest.Image,
                    src => string.IsNullOrEmpty(src.Image) ? "https://source.unsplash.com/random/800x600" : src.Image);

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

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                entity.User = user;
                entity.Image = "https://source.unsplash.com/random/800x600";

                await _context.Mixes.AddAsync(entity);
                await _context.SaveChangesAsync();

                var response = entity.Adapt<MixDTO>();
                return CreatedAtAction(nameof(Get), new {id = response.Id}, response);
            } catch (DbUpdateException ex) {
                _logger.LogError("Error creating mix");
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
