using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers;

[Route("[controller]")]
public class PingController(UserManager<MixyBoosUser> userManager, ILogger<PingController> logger)
  : _Controller(userManager,
    logger) {
  [HttpGet]
  public string Get() {
    return $"Pong{Environment.NewLine}";
  }
}
