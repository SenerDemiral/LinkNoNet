global using Microsoft.AspNetCore.Components.Server.Circuits;
global using System.Collections.Concurrent;
global using TntMud.Login;
using DataLibrary;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System.Diagnostics;
using TntMud;
//using Microsoft.AspNetCore.SignalR.Protocols.MessagePack
using TntMud.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
    //.AddHubOptions(options => {
    //    options.MaximumReceiveMessageSize = 65_536;  // Default 32KB: 32_768
    //});

builder.Services.AddImageSharp();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();

builder.Services.AddSingleton<IDataAccess, FBDataAccess>();
builder.Services.AddSingleton<IDataSet, DataSet>();

builder.Services.AddScoped<AppState>();
builder.Services.AddSingleton<ICircuitUserService, CircuitUserService>();
builder.Services.AddScoped<CircuitHandler>((sp) => new CircuitHandlerService(sp.GetRequiredService<ICircuitUserService>()));


var app = builder.Build();
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseImageSharp();
app.UseStaticFiles(); // Serve files from wwwroot
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.ContentRootPath, "uploads"))
//});
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//using (var scope = app.Services.CreateScope())
//{
//    var getService = scope.ServiceProvider.GetRequiredService<IDataSet>();

//    var aaa = await getService.UtLblSrch(fndAnd: "21,40", fndOr: "");
//    Stopwatch sw = Stopwatch.StartNew();
//    for (int i = 0; i < 100_000; i++)
//    {
//        aaa = await getService.UtLblSrch(fndAnd: "2,40,66", fndOr: "");
//    }
//    sw.Stop();
//    Console.WriteLine($"UTset: {sw.ElapsedMilliseconds}ms {sw.ElapsedTicks}tick");
//}

app.Run();