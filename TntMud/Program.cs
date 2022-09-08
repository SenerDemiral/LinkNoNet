global using Microsoft.AspNetCore.Components.Server.Circuits;
global using System.Collections.Concurrent;
global using TntMud.Login;
using DataLibrary;
using Microsoft.AspNetCore.HttpOverrides;
using MudBlazor.Services;
using SixLabors.ImageSharp.Web.DependencyInjection;
using TntMud;
//using Microsoft.AspNetCore.SignalR.Protocols.MessagePack
using TntMud.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
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
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();