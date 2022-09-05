using RepeatingTask;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<RepeatingService>();
builder.Services.AddSingleton<BasicModel, BasicModel>();    // Ornek
builder.Services.AddSingleton<GetFirstToken, GetFirstToken>();
builder.Services.AddSingleton<GetNewToken, GetNewToken>();
builder.Services.AddSingleton<GetOrders, GetOrders>();

var app = builder.Build();

// 5 numarali Magaza icin IdeaSoft a bildirilen redirect_url api.linkNo.net/auth/5
// Basarili 
// api.linkNo.net/auth/5?state=2b33fdd45jbevd6nam&code=Q0ODMI2OGYjMjBkN2mJmYzNkOTE4gzMGRhZDZTcykyOGQ3M2M2YTU2ZGVlMzE2MzB2MYkc5NWzE0ZWNiYjI2MA
// Basarisiz
// api.linkNo.net/auth/5?error=access_denied&error_description=The%20user%20denied%20access%20to%20your%20application&state=2b33fdd45jbevd6nam
app.MapGet("/auth/{mgzId:int}", async (int mgzId, string state, string? code, string? error, string? error_description) =>
{
    if(error != null)
    {
        // Log error & error_description
    }
    else
    {
        // gelen code'u kullanarak ilk Token'i al
        using (var scope = app.Services.CreateScope())
        {
            var sampleService = scope.ServiceProvider.GetRequiredService<GetFirstToken>();
            await sampleService.OnGet(mgzId: mgzId, mgzUri: "", client_id: "", client_secret: "", code: code);
        }
    }
    return $"OK.{mgzId}";
});

app.Run();

