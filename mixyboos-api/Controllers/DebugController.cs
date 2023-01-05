using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data.DTO;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class DebugController : _Controller {
        public DebugController(ILogger<DebugController> logger) : base(logger) { }

        [HttpGet]
        public async Task<DebugDTO> GetOsInfo() {
            return await Task.FromResult(new DebugDTO {
                LibVersion = Assembly
                    .GetEntryAssembly()?
                    .GetCustomAttribute<TargetFrameworkAttribute>()?
                    .FrameworkName,
                OSVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            });
        }
    }
}
