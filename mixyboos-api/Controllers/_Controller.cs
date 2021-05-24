using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MixyBoos.Api.Controllers {
    public class _Controller : Controller {
        protected readonly ILogger _logger;

        public _Controller(ILogger logger) {
            _logger = logger;
        }
    }
}
