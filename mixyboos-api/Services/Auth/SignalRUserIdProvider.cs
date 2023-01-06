using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace MixyBoos.Api.Services.Auth;

public class CustomEmailProvider : IUserIdProvider {
    public virtual string GetUserId(HubConnectionContext connection) {
        return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
}
