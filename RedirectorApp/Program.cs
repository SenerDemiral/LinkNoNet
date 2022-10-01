using Microsoft.AspNetCore.HttpOverrides;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using HashidsNet;
using Microsoft.Extensions.DependencyInjection;
using RedirectorApp;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("C:\\AspNetConfig\\LinkNoNet.json",
                       optional: true,
                       reloadOnChange: true);

// Add services to the container.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddSingleton<IDataAccess, FBDataAccess>();

// Simdilik kullanilmiyor
builder.Services.AddSingleton<IHashids>(_ =>
{
    var salt = builder.Configuration["HashIds:Salt"];
    var length = Int32.Parse(builder.Configuration["HashIds:Length"]!);
    return new Hashids(salt, length);
});

builder.Services.AddSingleton<DenemeA>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/dnm", (IHashids hashIds) =>
{
    var aaa = hashIds.Encode(555);
    var bbb = hashIds.DecodeSingle(aaa);
});

app.MapGet("/dnma", (DenemeA da) =>
{
    var aaa = da.hashIds0.Encode(5555555);
    var bbb = da.hashIds0.DecodeSingle(aaa);

    aaa = da.hashIds5.Encode(555);
    bbb = da.hashIds5.DecodeSingle(aaa);

    aaa = da.hashIds8.Encode(555);
    bbb = da.hashIds8.DecodeSingle(aaa);

    aaa = da.hashIds11.Encode(555);
    bbb = da.hashIds11.DecodeSingle(aaa);
});

// http://rdr.linkno.net/cst?mt={mtId}&tt={ttId}  Customer/Musteri bu link ile Magazaya gidecek
app.MapGet("/cst", (HttpRequest request, int? mt, int? tt, IDataAccess fb) =>
{
    if (mt is null || tt is null)
    {
        return Results.BadRequest("Hata");
    }
    var url = request.GetDisplayUrl();
    var host = request.Host.ToString();
    var rip = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var row = fb.StoreProc<Redirect_Op, dynamic>("Redirect_Op(@mt, @tt, @rip, @url, @host)", new { mt = mt, tt = tt, rip = rip, url = url, host = host });
    if (row.Ok == "H")
        return Results.NotFound("Hata");

    return Results.Redirect(row.RdrUri, false, true);

    // f f  302 Found               should not be cached
    // t f  301 MovedPermanently
    // f t  307 TemporaryRedirect   should not be cached ****
    // t t  308 PermanentRedirect
    //return Results.NotFound();
});

app.Run();

internal record Redirect_Op(string Ok, string RdrUri);
