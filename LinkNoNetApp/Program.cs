using DataLibrary;
using LinkNoNetApp;
using LinkNoNetApp.Data;
//using LinkNoNetApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("C:\\AspNetConfig\\LinkNoNet.json",
                       optional: true,
                       reloadOnChange: true);

//builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
//{
//    _ = config.AddJsonFile("C:\\AspNetConfig\\LinkNoNet.json",
//                       optional: true,
//                       reloadOnChange: true);
//});

builder.Services.AddImageSharp();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddSingleton<IPubs, Pubs>();
builder.Services.AddSingleton<UsrHub>();
builder.Services.AddSingleton<DataHub>();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IDataAccess, FBDataAccess>();
builder.Services.AddScoped<ClipboardService>();

//builder.Services.AddMudServices();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var app = builder.Build();

//foreach (var c in builder.Configuration.AsEnumerable())
//{
//    if (c.Key.StartsWith("Hash") || c.Key.StartsWith("Mail"))
//    Console.WriteLine(c.Key + " = " + c.Value);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseImageSharp();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet("/pubsDnm/{who}/{msg}", (IPubs pubs, string who, string msg) =>
{
    pubs.AdmMsgRaise(who, msg);
});

app.MapGet("/DataHub/{etid}/{utid}/{info}", async (DataHub dHub, int etid, int utid, string info) =>
{
    await dHub.ChatInit(etid, utid);
    await dHub.ChatAdd(etid, utid, info);
});

app.Run();