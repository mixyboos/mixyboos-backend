using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixyBoos.Api.Data.DTO;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class DebugController : Controller {
        [HttpGet]
        public async Task<DebugDTO> GetOsInfo() {
            return await Task.FromResult<DebugDTO>(new DebugDTO {
                NetCoreVersion = Assembly
                    .GetEntryAssembly()?
                    .GetCustomAttribute<TargetFrameworkAttribute>()?
                    .FrameworkName,
                OSVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            });
        }
    }
}
