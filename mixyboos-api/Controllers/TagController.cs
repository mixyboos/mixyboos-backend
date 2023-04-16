using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class TagController : _Controller {
    private readonly MixyBoosContext _context;
    private readonly UserManager<MixyBoosUser> _userManager;

    public TagController(MixyBoosContext context,
        UserManager<MixyBoosUser> userManager,
        ILogger<TagController> logger) : base(logger) {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("search")]
    public async Task<List<string>> DoSearch([FromQuery] string query) {
        var results = await _context
            .Tags
            .Where(t => t.TagName.Contains(query))
            .Select(t => t.TagName)
            .ToListAsync();
        return results;
    }
}
