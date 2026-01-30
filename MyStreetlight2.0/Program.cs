using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs;
using MyStreetlight2._0.Filters.PasswordCheckFilter;
using MyStreetlight2._0.Services.CommonDataService;
using MyStreetlight2._0.Services.GatewayService;
using MyStreetlight2._0.Services.LogServices;
using MyStreetlight2._0.Services.MaintenanceServices;
using MyStreetlight2._0.Services.StreetlightService;
using MyStreetlight2._0.Services.UserService;
using MyStreetlight2._0.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(config =>
{
    // Apply a global authorization filter
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});
//builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";   // redirect if not logged in
        options.LogoutPath = "/Auth/Logout"; // logout endpoint
        //options.AccessDeniedPath = "/account/denied"; // forbidden access
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // cookie expiration
        options.SlidingExpiration = true; // refresh expiration on activity
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.Name = "StreetlightToken"; // default name
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<PasswordCheckFilter>();
builder.Services.AddScoped<CookieDecoder>();

//builder.Services.AddHostedService<LightUpdateService>();

// //For update ligth data from web(temporary)
// builder.Services.AddHostedService<RamdomUpdate>();
// builder.Services.AddSingleton<LightUpdateFlag>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStreetlightService, StreetlightService>();
builder.Services.AddScoped<ICommonDataService, CommonDataService>();
builder.Services.AddScoped<IGatewayService, GatewayService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gateway}/{action=Index}/{id?}");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
