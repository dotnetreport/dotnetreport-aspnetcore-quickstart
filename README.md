# Add self-service ad hoc reporting to an ASP.NET Core app (quickstart)

A minimal, working example of embedding a drag-and-drop **self-service report builder** into an existing ASP.NET Core application using [Dotnet Report](https://dotnetreport.com). End users get to build their own reports, charts and dashboards over your database — without a developer hand-coding each report.

This repo contains only the pieces **you** change. Everything else (the report builder UI, API controllers, views and scripts) is installed by the NuGet package.

> The Dotnet Report report-builder front-end is source-available on GitHub: https://github.com/dotnetreport/dotnetreport

## What you get

- A `/dotnetsetup` admin screen that connects to your database and lists its tables/views
- A `/dotnetreport` end-user report builder: pick tables and columns, add filters, group, chart, drill down, export, schedule
- Everything runs inside **your** app, on **your** database — the design/metadata lives in your Dotnet Report account

## Prerequisites

- .NET 6 or later (the upstream repo currently targets .NET 10)
- A SQL database your app can reach (SQL Server, PostgreSQL, MySQL, Oracle, and more are supported)
- A free Dotnet Report account for the three API tokens: https://dotnetreport.com

## 1. Install the package

```bash
dotnet add package DotnetReport
```

(.NET Framework MVC apps use `DotnetReport.Mvc`; Web Forms apps use `DotnetReport.aspx`.)

## 2. Add the configuration

Add a `dotNetReport` section and a `ConnectionKey` connection string to `appsettings.json` (see [`appsettings.sample.json`](appsettings.sample.json)):

```json
{
  "dotNetReport": {
    "accountapiurl": "https://dotnetreport.com/portal/api",
    "apiurl": "https://dotnetreport.com/api",
    "accountApiToken": "YOUR-PUBLIC-ACCOUNT-API-TOKEN",
    "dataconnectApiToken": "YOUR-DATA-CONNECT-API-TOKEN",
    "privateApiToken": "YOUR-PRIVATE-API-TOKEN"
  },
  "ConnectionStrings": {
    "ConnectionKey": "Server=.;Database=YourDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

The three tokens come from your Dotnet Report account. **Never commit real tokens** — use user secrets or environment variables in real projects:

```bash
dotnet user-secrets set "dotNetReport:privateApiToken" "..."
```

## 3. Register the services the report builder needs

The report builder is plain MVC (controllers + Razor views + jQuery/Knockout scripts), so it needs controllers-with-views, session, an `HttpClient`, and the `IHttpContextAccessor`. See [`Program.cs`](Program.cs) for the complete minimal host — the key lines are:

```csharp
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// ...

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

## 4. Connect your database

Run the app and open **`/dotnetsetup`**. This admin/developer screen connects using your `ConnectionKey` connection string and lists every table and view. Pick the ones you want end users to report on and save.

## 5. Build a report

Open **`/dotnetreport`**. Your users can now create reports and dashboards over the tables you exposed — with filters, grouping, charts, drill-down, export and scheduling — no code required.

## Locking it down (recommended next steps)

- The report routes are decorated with `[Authorize]`; wire them into your existing authentication so only signed-in users reach them.
- Pass your real user/tenant context into Dotnet Report so reports, folders and data are scoped per user and per tenant → see the companion repo **[dotnetreport-multitenant-rls](https://github.com/dotnetreport/dotnetreport-multitenant-rls)** for row-level security driven by claims.
- Turn on scheduled email delivery of reports (PDF/Excel) → see **[dotnetreport-scheduled-reports](https://github.com/dotnetreport/dotnetreport-scheduled-reports)**.

## Related

- Getting started guide: https://dotnetreport.com/blogs/getting-started-with-dotnet-report/
- Knowledge base: https://dotnetreport.com/docs
- Live demo: https://dotnetreport.com/demo/Report

---

Maintained by the Dotnet Report team. Issues and PRs welcome.
