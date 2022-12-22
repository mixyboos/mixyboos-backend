using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MixyBoos.Api.Data.Seeders {
    public class DbInitializer : IDbInitializer {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory) {
            this._scopeFactory = scopeFactory;
        }

        public async Task Initialize() {
            using var serviceScope = _scopeFactory.CreateScope();
            await using var context = serviceScope.ServiceProvider.GetService<MixyBoosContext>();
            await context.Database.MigrateAsync();
        }

        public async Task SeedData() {
            using var serviceScope = _scopeFactory.CreateScope();
            await using var context = serviceScope.ServiceProvider.GetService<MixyBoosContext>();
            var logger = serviceScope.ServiceProvider.GetService<ILogger<DbInitializer>>();
            var seeder = new TestData(context, logger);

            if (!await context.Users.AnyAsync()) {
                var users = await seeder.GetTestUsers();
                foreach (var user in users) {
                    if (!context.Users.Any(u => u.UserName == user.UserName)) {
                        var password = new PasswordHasher<MixyBoosUser>();
                        var hashed = password.HashPassword(user, "SVqVKJWZh5dIaM7JsNY1h0E/xbzPCD7y7Veedxa1Q/k=");
                        user.PasswordHash = hashed;

                        var userStore = new UserStore<MixyBoosUser>(context);
                        await userStore.CreateAsync(user);
                    }
                }
            }

            if (!await context.Mixes.AnyAsync()) {
                var mixes = await seeder.GetTestMixes();
                try {
                    await context.Mixes.AddRangeAsync(mixes);
                } catch (Exception e) {
                    logger.LogError(e.Message);
                }
            }

            // if (!await context.LiveShows.AnyAsync()) {
            //     var shows = await seeder.GetTestShows();
            //     try {
            //         await context.LiveShows.AddRangeAsync(shows);
            //         await context.SaveChangesAsync();
            //     } catch (Exception e) {
            //         logger.LogError(e.Message);
            //     }
            // }

            await context.SaveChangesAsync();
        }
    }
}
