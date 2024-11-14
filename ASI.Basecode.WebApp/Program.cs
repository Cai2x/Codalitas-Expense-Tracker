using System.IO;
using ASI.Basecode.Data;
using ASI.Basecode.WebApp;
using ASI.Basecode.WebApp.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var appBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
});

appBuilder.Configuration.AddJsonFile("appsettings.json",
    optional: true,
    reloadOnChange: true);

appBuilder.WebHost.UseIISIntegration();

appBuilder.Logging
    .AddConfiguration(appBuilder.Configuration.GetLoggingSection())
    .AddConsole()
    .AddDebug();

var configurer = new StartupConfigurer(appBuilder.Configuration);
configurer.ConfigureServices(appBuilder.Services);

var app = appBuilder.Build();

configurer.ConfigureApp(app, app.Environment);

// General route for CRUD-like actions
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Explicit routes for specific actions
app.MapControllerRoute(
    name: "expenseEdit",
    pattern: "Expense/Edit/{id}",
    defaults: new { controller = "Expense", action = "Edit" });

app.MapControllerRoute(
    name: "categoryEdit",
    pattern: "Category/Edit/{id}",
    defaults: new { controller = "Category", action = "Edit" });

app.MapControllers();
app.MapRazorPages();

// Run application
app.Run();
