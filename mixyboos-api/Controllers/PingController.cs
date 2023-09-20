using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class PingController : _Controller {
  public PingController(ILogger<PingController> logger) : base(logger) { }

  [HttpGet]
  public string Get() {
    return $"Pong{Environment.NewLine}";
  }
}
