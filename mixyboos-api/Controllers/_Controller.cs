using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers;

public class _Controller : Controller {
  protected readonly UserManager<MixyBoosUser> _userManager;
  protected readonly ILogger _logger;

  private MixyBoosUser _currentUser;

  protected _Controller(UserManager<MixyBoosUser> userManager, ILogger logger) {
    _userManager = userManager;
    _logger = logger;
  }

  protected async Task<MixyBoosUser> GetCurrentUserAsync() {
    if (_currentUser != null)
      return _currentUser;

    if (User?.Identity?.IsAuthenticated == true) {
      _currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
    }

    return _currentUser;
  }

  public MixyBoosUser CurrentUser => GetCurrentUserAsync().Result;
}
