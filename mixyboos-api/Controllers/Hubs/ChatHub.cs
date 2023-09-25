using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers.Hubs;

[Authorize]
public class ChatHub : Hub {
    private readonly MixyBoosContext _context;
    private readonly UserManager<MixyBoosUser> _userManager;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(MixyBoosContext context, UserManager<MixyBoosUser> userManager, ILogger<ChatHub> logger) {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    [HubMethodName("JoinShow")]
    public async Task<string> JoinShow(string user, string show) {
        await JoinRoom(show).ConfigureAwait(false);
        return show;
    }

    public async Task<bool> SendMessage(string user, string message, string showId) {
        _logger.LogInformation("New chat message - From: {From}, Show: {Show}, Message: {Message}",
            user, message, showId);
        var fromUser = await _userManager.FindByNameAsync(Context.User.Identity.Name);
        var toUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(Guid.Parse(user)));
        var show = await _context.LiveShows.FirstOrDefaultAsync(u => u.Id.Equals(Guid.Parse(showId)));
        if (fromUser is null || toUser is null || show is null) {
            _logger.LogError("Unable to resolve details for message");
            return false;
        }

        try {
            var chat = await _context.ShowChat.AddAsync(
                new ShowChat {
                    FromUser = fromUser,
                    ToUser = toUser,
                    Show = show,
                    DateSent = DateTime.UtcNow,
                    Message = message
                });
            await _context.SaveChangesAsync();
            var response = chat.Entity.Adapt<ShowChatDTO>();
            await Clients.Group(showId).SendAsync(
                    "ReceiveMessage",
                    response)
                .ConfigureAwait(true);
            return true;
        } catch (DbUpdateException e) {
            _logger.LogError("Unable to create show record\n{Error}", e.Message);
        }

        return false;
    }

    public Task JoinRoom(string roomName) {
        return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public Task LeaveRoom(string roomName) {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
}
