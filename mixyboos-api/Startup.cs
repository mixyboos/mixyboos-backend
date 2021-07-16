using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Seeders;
using MixyBoos.Api.Services.Auth;
using MixyBoos.Api.Services.Helpers.Audio;
using MixyBoos.Api.Services.Jobs;
using MixyBoos.Api.Services.Workers;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.EntityFrameworkCore;
using Quartz;

namespace MixyBoos.Api {
    public class Startup {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
            services.AddScoped<IUserClaimsPrincipalFactory<MixyBoosUser>, ClaimsPrincipalFactory>();
            services.AddSingleton<IAudioFileConverter, AudioFileConverter>();

            services.AddDbContext<MixyBoosContext>(options => {
                options
                    .UseNpgsql(_configuration.GetConnectionString("MixyBoos"))
                    .UseSnakeCaseNamingConvention()
                    .UseOpenIddict();
            });

            services.AddIdentity<MixyBoosUser, IdentityRole>()
                .AddEntityFrameworkStores<MixyBoosContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            services.AddSignalR();

            // .AddCore()
            // .AddServer(options => {
            //
            services.AddOpenIddict()
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
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo");

                    if (_env.IsDevelopment()) {
                        options
                            .AddEphemeralEncryptionKey()
                            .AddEphemeralSigningKey()
                            .DisableAccessTokenEncryption();

                        options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                    }

                    options.RegisterScopes("api");
                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough();
                }).AddValidation(options => {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.AddOpenIddict()
                .AddCore()
                .UseEntityFrameworkCore(options => {
                    options.UseDbContext<MixyBoosContext>();
                });

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "API__AUTH";
                options.Cookie.HttpOnly = true;
                options.Cookie.Domain = "*.mixyboos.com";
                options.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddHostedService<OpenIdDictWorker>();
            // services.AddHostedService<UploadFileProcessor>();
            services.AddQuartz(q => {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                var jobKey = new JobKey("ProcessUploadedAudioJob");
                q.AddJob<ConvertAudioJob>(opts => {
                    opts.WithIdentity(jobKey).StoreDurably();
                });
            });

            services
                .AddAuthentication(options => {
                    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                });

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

            app.UseCors(builder => builder
                .WithOrigins("https://dev.mixyboos.com:3000")
                .WithOrigins("http://dev.mixyboos.com:3000")
                .WithOrigins("https://mixyboos.com")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<LiveHub>("/hubs/live");
                endpoints.MapHub<Chat>("/hubs/chat");
            });

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            dbInitializer.Initialize();
            dbInitializer.SeedData();
        }
    }
}
