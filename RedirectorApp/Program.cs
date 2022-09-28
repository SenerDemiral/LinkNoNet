using Microsoft.AspNetCore.HttpOverrides;
using DataLibrary;
using FirebirdSql.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using HashidsNet;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

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
    var length = Int32.Parse(builder.Configuration["HashIds:Salt"]);
    return new Hashids(builder.Configuration["HashIds:Salt"], length);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// http://rdr.linkno.net/cst?mt={mtId}&tt={ttId}  Customer/Musteri bu link ile Magazaya gidecek
app.MapGet("/cst", (HttpRequest request, int ? mt, int? tt, [FromServices] IDataAccess fb) =>
{
    if (mt is null || tt is null) 
    { 
        return Results.BadRequest("Hata"); 
    }
    var url = request.GetDisplayUrl();
    var host = request.Host.ToString();
    var rip = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var row = fb.StoreProc<Redirect_Op, dynamic>("Redirect_Op(@mt, @tt, @rip, @url, @host)", new {mt=mt, tt=tt, rip=rip, url=url, host=host});
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
