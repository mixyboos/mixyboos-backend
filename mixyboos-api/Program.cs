using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Auth;
using MixyBoos.Api.Services.Helpers;
using MixyBoos.Api.Services.Helpers.Audio;
using MixyBoos.Api.Services.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var instance = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(instance);

builder.CreateLogger(builder.Configuration);

builder.Services.Configure<DbScaffoldOptions>(
  builder.Configuration.GetSection("DbScaffoldOptions")
);
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddTransient<IEmailSender, ARMMailSender>();
builder.Services.AddSingleton<IAudioFileConverter, AudioFileConverter>();
builder.Services.AddSingleton<IUserIdProvider, CustomEmailProvider>();
builder.Services.AddSingleton<ImageCacher>();
builder.Services.AddSingleton<ImageHelper>();
builder.Services.AddSingleton<IFileProvider, PhysicalFileProvider>(_ =>
  new PhysicalFileProvider(
    builder.Configuration["ImageProcessing:ImageRootFolder"] ?? ".pn-cache"));

builder.Services.AddDbContext<MixyBoosContext>(options =>
  options
    .UseNpgsql(builder.Configuration.GetConnectionString("MixyBoos"), options => {
      options
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        .MigrationsHistoryTable("migrations", "sys");
    }).EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddMixyboosAuthentication(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.Configure<RouteOptions>(options => {
  options.LowercaseUrls = true;
});
builder.Services.LoadScheduler();

var app = builder.Build();


// Apply pending migrations
using (var scope = app.Services.CreateScope()) {
  var dbContext = scope.ServiceProvider
    .GetRequiredService<MixyBoosContext>();

  // Here is the migration executed
  dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors(corsBuilder => corsBuilder
  .WithOrigins("http://localhost:3000")
  .WithOrigins("https://mixyboos.dev.fergl.ie:3000")
  .WithOrigins("http://mixyboos.dev.fergl.ie:3000")
  .WithOrigins("https://www.mixyboos.com")
  .WithOrigins("https://mixyboos.com")
  .AllowCredentials()
  .AllowAnyHeader()
  .AllowAnyMethod()
);

app.UseSignalRHubs();
app.UseSerilogRequestLogging();
app.MapGroup("/auth")
  .MapIdentityApi<MixyBoosUser>()
  .WithTags("Auth");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
