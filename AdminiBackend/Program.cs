using AdminiBackend.API;
using AdminiBackend.Services;
using AdminiDomain;
using AdminiDomain.Entities;
using AdminiDomain.Services;
using AdminiInfrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Net;


Log.Logger = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.File("log.txt", restrictedToMinimumLevel: LogEventLevel.Warning)
  .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
  .CreateLogger();

Log.Information("Starting up");

try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.Host.UseSerilog();

  builder.Services.AddRazorPages();
  builder.Services.AddDbContext<AdminiContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("AdminiContext")
      ?? throw new InvalidOperationException("Connection string 'AdminiContext' not found."),
      x => x.MigrationsAssembly("AdminiInfrastructure")));
  builder.Services
    .AddHttpContextAccessor()
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = new PathString("/Panel/Login");
      options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });
  builder.Services
    .AddScoped<IRepository, RepositorySqlServer>()
    .AddScoped<AuthService>()
    .AddScoped<NoteService>()
    .AddScoped<NoteContentService>()
    .AddScoped<NoteFileService>()
    .AddScoped<TagService>()
    .AddScoped<UserService>();

  var app = builder.Build();

  using (var scope = app.Services.CreateScope())
  {
    using var context = scope.ServiceProvider.GetRequiredService<AdminiContext>();
    try
    {
      var adminUser = context.Users.FirstOrDefault(user => user.Role == UserRoles.Admin);
      if (adminUser is null)
      {
        await context.AddAsync(new User()
        {
          Name = "admini",
          Password = CryptographyService.Encrypt("admini"),
          Role = UserRoles.Admin
        });
        await context.SaveChangesAsync();
      }
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Failed to create admin user");
    }
  }

  PublicAPI.Register(app);

  if (app.Environment.IsDevelopment())
  {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
  }

  app.Use(async (context, next) =>
  {
    await next();
    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
    {
      context.Request.Path = "/index.html";
      await next();
    }
  });

  app.UseHttpsRedirection();
  app.UseDefaultFiles();
  app.UseStaticFiles();

  app.UseRouting();
  app.UseAuthentication();
  app.UseAuthorization();

  app.MapRazorPages();

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Unhandled exception");
}
finally
{
  Log.Information("Shut down complete");
  Log.CloseAndFlush();
}
