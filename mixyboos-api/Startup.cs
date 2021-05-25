using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Seeders;
using MixyBoos.Api.Services.Workers;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddDbContext<MixyBoosContext>(options => {
                options.UseNpgsql(Configuration.GetConnectionString("MixyBoosContext"));
                options.UseOpenIddict();
            });

            // services.AddDbContext<MixyBoosContext>(options => {
            //     options.UseInMemoryDatabase(nameof(MixyBoosContext));
            //     options.UseOpenIddict();
            // });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MixyBoosContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });


            services.AddOpenIddict()
                .AddCore(options => {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<MixyBoosContext>();
                })
                .AddServer(options => {
                    options
                        .AllowPasswordFlow()
                        .AllowClientCredentialsFlow()
                        .AllowAuthorizationCodeFlow()
                        .AcceptAnonymousClients()
                        .AllowRefreshTokenFlow()
                        .RequireProofKeyForCodeExchange()
                        .AcceptAnonymousClients();

                    options
                        .SetAuthorizationEndpointUris("/connect/authorize")
                        .SetTokenEndpointUris("/connect/token");

                    // options
                    //     .AddEphemeralEncryptionKey()
                    //     .AddEphemeralSigningKey()
                    //     .DisableAccessTokenEncryption();
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.RegisterScopes("api");
                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough();
                }).AddValidation(options => {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });
            ;

            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddHostedService<OpenIdDictWorker>();
            services
                .AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "MixyBoos.Api", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MixyBoos Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors(builder => {
                builder.WithOrigins("http://dev.mixyboos.com:3000");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            dbInitializer.Initialize();
            dbInitializer.SeedData();
        }
    }
}
