using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;

namespace MixyBoos.Api.Controllers {
    [Route("[controller]")]
    public class MixController : _Controller {
        private readonly MixyBoosContext _context;

        public MixController(MixyBoosContext context,
            ILogger<MixController> logger) : base(logger) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MixDTO>>> Get() {
            var mixes = await _context.Mixes.Include(m => m.User).ToListAsync();
            var result = mixes.Adapt<List<MixDTO>>();

            return Ok(result);
        }
    }
}
