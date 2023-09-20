﻿using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using MixyBoos.Api.Data.Seeders;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Auth;
using MixyBoos.Api.Services.Helpers;
using MixyBoos.Api.Services.Helpers.Audio;


var builder = WebApplication.CreateBuilder(args);

var instance = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(instance);

builder.WebHost.ConfigureKestrel(options => {
  var pemFile = builder.Configuration["SSL:PemFile"];
  var keyFile = builder.Configuration["SSL:KeyFile"];
  if (string.IsNullOrEmpty(pemFile) || string.IsNullOrEmpty(keyFile)) {
    return;
  }

  options.Listen(IPAddress.Any, 5001, listenOptions => {
    var certPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/fullchain.pem");
    var keyPem = File.ReadAllText("/etc/letsencrypt/live/dev.fergl.ie/privkey.pem");
    var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);
    listenOptions.UseHttps(x509);
  });
});


builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddTransient<IEmailSender, ARMMailSender>();
builder.Services.AddSingleton<IAudioFileConverter, AudioFileConverter>();
builder.Services.AddSingleton<IUserIdProvider, CustomEmailProvider>();
builder.Services.AddSingleton<ImageCacher>();
builder.Services.AddSingleton<ImageHelper>();
builder.Services.AddSingleton<IFileProvider, PhysicalFileProvider>(_ =>
  new PhysicalFileProvider(
    builder.Configuration["ImageProcessing:ImageRootFolder"] ?? ".pn-cache"));

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<MixyBoosContext>(options =>
  options
    .UseNpgsql(builder.Configuration.GetConnectionString("MixyBoos"), options => {
      options
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        .MigrationsHistoryTable("migrations", "sys");
    }).EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services
  .AddIdentityCore<MixyBoosUser>()
  .AddEntityFrameworkStores<MixyBoosContext>()
  .AddApiEndpoints();

builder.Services.Configure<IdentityOptions>(options => {
  // Default Password settings.
  options.Password.RequireDigit = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
  options.Password.RequiredLength = 4;
  options.Password.RequiredUniqueChars = 0;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options => {
  options.LowercaseUrls = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGroup("/auth")
  .MapIdentityApi<MixyBoosUser>()
  .WithTags("Auth");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
