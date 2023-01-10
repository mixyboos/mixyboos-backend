using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
using MixyBoos.Api.Services.Startup;
using MixyBoos.Api.Services.Startup.Mapster;
using MixyBoos.Api.Services.Workers;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

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
            services.AddSingleton<IUserIdProvider, CustomEmailProvider>();

            //register the codepages (required for slugify)
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);

            services.AddDbContext<MixyBoosContext>(options => {
                options
                    .UseNpgsql(_configuration.GetConnectionString("MixyBoos"), pgoptions => {
                        pgoptions.MigrationsHistoryTable("migrations", "sys");
                    })
                    .UseSnakeCaseNamingConvention()
                    .UseOpenIddict();
                if (_env.IsDevelopment()) {
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddIdentity<MixyBoosUser, IdentityRole>()
                .AddEntityFrameworkStores<MixyBoosContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Email;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            services.RegisterMapsterConfiguration(_configuration);
            services.RegisterHttpClients(_configuration);

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

                    // if (_env.IsDevelopment()) {
                    //     options
                    //         .AddEphemeralEncryptionKey()
                    //         .AddEphemeralSigningKey()
                    //         .DisableAccessTokenEncryption();
                    //
                    //     options.AddDevelopmentEncryptionCertificate()
                    //         .AddDevelopmentSigningCertificate();
                    // } else {
                    //     options.AddEncryptionKey(new SymmetricSecurityKey(
                    //         Convert.FromBase64String(_configuration["Auth:SigningKey"] ?? string.Empty)));
                    //     options.AddSigningCertificate(SigningCertificateGenerator.CreateSigningCertificate());
                    //     options.AddEncryptionCertificate(SigningCertificateGenerator.CreateEncryptionCertificate());
                    // }
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey()
                        .DisableAccessTokenEncryption();

                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.RegisterScopes("api");
                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .DisableTransportSecurityRequirement();
                }).AddValidation(options => {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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
            services.LoadScheduler();
            services.AddSilkierQuartz(options => {
                options.VirtualPathRoot = "/jobs";
            }, authenticationOptions => {
                authenticationOptions.AccessRequirement =
                    SilkierQuartzAuthenticationOptions.SimpleAccessRequirement.AllowAnonymous;
            });


            services
                .AddAuthentication(options => {
                    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = "/auth/login";
                });

            services.Configure<IdentityOptions>(options => {
                // Default Password settings.
                //jp1QhhMTysXddk6LiYv2UUU3tVubLvJrjwpsYt1fkM0=

                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
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
                app.UseHttpsRedirection();
            }

            app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseCors(builder => builder
                .WithOrigins("https://mixyboos.dev.fergl.ie:3000")
                .WithOrigins("http://mixyboos.dev.fergl.ie:3000")
                .WithOrigins("https://www.mixyboos.com")
                .WithOrigins("https://mixyboos.com")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSilkierQuartz();
            // app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<DebugHub>("/hubs/debug");
                endpoints.MapHub<LiveHub>("/hubs/live");
                endpoints.MapHub<ChatHub>("/hubs/chat");
                endpoints.MapHub<UpdatesHub>("/hubs/updates");
            });

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MixyBoos Api v1"));

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            dbInitializer.Initialize();
            dbInitializer.SeedData();
        }
    }
}
