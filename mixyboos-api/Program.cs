using System;
using System.Security.Claims;
using System.Text;
using CrystalQuartz.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Data.Options;
using MixyBoos.Api.Data.Repositories;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Auth;
using MixyBoos.Api.Services.Helpers;
using MixyBoos.Api.Services.Helpers.Audio;
using MixyBoos.Api.Services.Helpers.IO;
using MixyBoos.Api.Services.Startup;
using MixyBoos.Api.Services.Startup.Mapster;
using Quartz;
using Serilog;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Using environment: {builder.Environment.EnvironmentName}");
Console.WriteLine(
  $"Reading configuration from: appsettings.{builder.Configuration.GetSection("Environment").Value}.json");
var instance = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(instance);

builder.CreateLogger(builder.Configuration);

builder.Services.Configure<DbScaffoldOptions>(
  builder.Configuration.GetSection("DbScaffoldOptions")
);
builder.Services.AddTransient<MixRepository>();
builder.Services.AddTransient<IRepository<MixLike>, Repository<MixLike>>();
builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddTransient<IEmailSender, ARMMailSender>();
builder.Services.AddTransient<IWaveformGenerator, WaveformGenerator>();
builder.Services.AddSingleton<IAudioFileConverter, AudioFileConverter>();
builder.Services.AddSingleton<IUserIdProvider, CustomEmailProvider>();
builder.Services.AddSingleton<ImageCacher>();
builder.Services.AddSingleton<ImageHelper>();
builder.Services.AddSingleton<IFileProvider, PhysicalFileProvider>(_ =>
  new PhysicalFileProvider("/"));
// builder.Configuration["ImageProcessing:ImageRootFolder"] ?? ".pn-cache"));

Console.WriteLine("About to show you the connection string");
Console.WriteLine(builder.Configuration.GetConnectionString("MixyBoos"));
Console.WriteLine("Showed you the connection string");

builder.Services.AddDbContext<MixyBoosContext>(options =>
  options
    .UseNpgsql(builder.Configuration.GetConnectionString("MixyBoos"), pgOptions => {
      pgOptions
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        .MigrationsHistoryTable("migrations", "sys");
    })
    .UseSnakeCaseNamingConvention()
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddMixyboosAuthentication(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddImaging(builder.Configuration);
builder.Services.Configure<RouteOptions>(options => {
  options.LowercaseUrls = true;
});
builder.Services.AddScheduler();

builder.Services.Configure<CookiePolicyOptions>(options => {
  options.CheckConsentNeeded = context => false;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
  var dbContext = scope.ServiceProvider
    .GetRequiredService<MixyBoosContext>();
  dbContext.Database.Migrate();
  builder.Services.RegisterMapsterConfiguration(builder.Configuration, scope.ServiceProvider);
}

if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors(corsBuilder => corsBuilder
  .WithOrigins("http://localhost:3000")
  .WithOrigins("https://mixyboos.dev.fergl.ie:3000")
  .WithOrigins("https://preview.mixyboos.com")
  .WithOrigins("https://www.mixyboos.com")
  .WithOrigins("https://mixyboos.com")
  .AllowCredentials()
  .AllowAnyHeader()
  .AllowAnyMethod()
);


app.UseSignalRHubs();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();


app.MapGroup("/auth")
  .MapIdentityApi<MixyBoosUser>()
  .WithTags("Auth");

app.MapGet("/pingauth", () => new {
    Ping = "Secure Pong"
  })
  .RequireAuthorization()
  .WithName("AuthPing");


var waveformDir = builder.Configuration["AudioProcessing:WaveformDir"];
if (DirectoryHelpers.ValidateDirectory(waveformDir)) {
  app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(waveformDir),
    RequestPath = new PathString("/waveforms")
  });
} else {
  throw new InvalidOperationException("Audio processing directory not found");
}

app.UseJobScheduler();

app.UseImageSharp();
app.Run();
