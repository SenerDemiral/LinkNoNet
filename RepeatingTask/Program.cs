using RepeatingTask;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDataAccess, FBDataAccess>();

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

// Biz manual olarak Browserdan asagidaki adrese gidip ACCEPT tusuna basinca
// BrowserLink: xxx.myideasoft.com/admin/user/auth?client_id=<gir>&response_type=code&state=<bizim verririz, response.state ile kontrol et>&redirect_uri=http://api.linkno.net/auth/{mgzId}
// Her bir Magaza icin bu browserLink'i hazirla. Bu islem bir kere yapilir
// Idesoft code bildirmek icin redirect_uri ile bildirdigimiz burayi cagirir
// Burasi da code kullanarak ilk Token lari alip DB ye kaydeder

// denemeURL: ... auth/21?state=AAAAAAAAAAAAA
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
            var getFirstTokenService = scope.ServiceProvider.GetRequiredService<GetFirstToken>();

            await getFirstTokenService.OnGet(mgzId, code!);
        }
    }
    return $"OK.{mgzId}";
});

app.Run();

