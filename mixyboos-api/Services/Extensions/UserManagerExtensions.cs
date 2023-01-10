#nullable enable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data;

namespace MixyBoos.Api.Services.Extensions;

public static class UserManagerExtensions {
    public static async Task<MixyBoosUser?> FindBySlugWithFollowingAsync(this UserManager<MixyBoosUser> userManager,
        string name) {
        var user = await userManager
            .Users
            .Include(u => u.Following)
            .Include(u => u.Followers)
            .SingleOrDefaultAsync(x => x.Slug.Equals(name));

        return user;
    }

    public static async Task<MixyBoosUser?> FindByNameWithFollowingAsync(this UserManager<MixyBoosUser> userManager,
        string name) {
        var user = await userManager
            .Users
            .Include(u => u.Following)
            .Include(u => u.Followers)
            .SingleOrDefaultAsync(x => x.UserName != null && x.UserName.Equals(name));

        return user;
    }

    public static async Task<MixyBoosUser?> FindBySlugAsync(this UserManager<MixyBoosUser> userManager,
        string slug) {
        var user = await userManager
            .Users
            .SingleOrDefaultAsync(x => x.Slug.Equals(slug));
        return user;
    }

    public static async Task<MixyBoosUser?> FindByStreamKeyAsync(this UserManager<MixyBoosUser> userManager,
        string streamKey) {
        var user = await userManager
            .Users
            .SingleOrDefaultAsync(x => x.StreamKey.Equals(streamKey));
        return user;
    }
}
