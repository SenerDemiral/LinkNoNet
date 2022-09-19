using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}



app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet("/t1", (HttpRequest request, HttpResponse response) =>
{
    return Results.Redirect("https://medlab.myideasoft.com?ref=999", true, true);
    // f f  302
    // t f  301
    // f t  307
    // t t  308
    //return Results.NotFound();
});

app.Run();
