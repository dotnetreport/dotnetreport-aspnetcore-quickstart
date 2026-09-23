// Minimal ASP.NET Core host for the Dotnet Report self-service report builder.
//
// This mirrors the services and middleware the report builder relies on. Drop the
// relevant lines into your existing Program.cs if you already have an app; the
// order of the app.Use* calls matters (routing -> auth -> session -> endpoints).
//
// Install the package first:  dotnet add package DotnetReport
// Then configure appsettings.json (see appsettings.sample.json) and browse to:
//   /dotnetsetup   -> connect your database and choose tables/views
//   /dotnetreport  -> the end-user report builder

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// The report builder is classic MVC (controllers + Razor views + scripts).
services.AddControllersWithViews();

// Used by the Dotnet Report API controllers to talk to the reporting service.
services.AddHttpClient();
services.AddHttpContextAccessor();

// User/role context for the report builder is kept in session.
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Any authentication scheme works; the report routes are [Authorize]-protected.
// Replace this with your existing auth (ASP.NET Core Identity, OpenID Connect, etc.).
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // serves the report builder's scripts/CSS

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();       // must come before the endpoints that read session

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
