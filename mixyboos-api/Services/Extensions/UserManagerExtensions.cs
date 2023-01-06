#nullable enable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data;

namespace MixyBoos.Api.Services.Extensions;

public static class UserManagerExtensions {
    public static async Task<MixyBoosUser?> FindByStreamKeyAsync(this UserManager<MixyBoosUser> userManager,
        string streamKey) {
        var user = await userManager
            .Users
            .SingleOrDefaultAsync(x => x.StreamKey.Equals(streamKey));
        return user;
    }
}
