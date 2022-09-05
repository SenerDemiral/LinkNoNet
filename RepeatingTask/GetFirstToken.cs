using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetFirstToken
{
    private readonly IHttpClientFactory _httpClient;
    public TokenModel Token { get; set; }

    public GetFirstToken(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory!;
    }

    /*
     http://www.ideashopgiyim.com/oauth/v2/token
    ?grant_type=authorization_code
    &client_id=7_7d67dc7597f034d63775c1d9ae5d9ac7f5750197f
    &client_secret=1sowg0oogc4wg4w4o4gh4va57gggwskkgo08m44ksog8kmu88o
    &code=Q0ODMI2OGYjMjBkN2mJmYzNkOTE4gzMGRhZDZTcykyOGQ3M2M2YTU2ZGVlMzE2MzB2MYkc5NWzE0ZWNiYjI2MA
    &redirect_uri=http%3A//client.example-app.com%2Fauth%2F

    response
    {
    "access_token": "OGE3MzkxNDlhOGY0M2RjZWM1MWI2MWIxZjRmNDJhZThiOGJkOWZlMWIwNzVkYWFlOWFiMDAxYzkwMDlmMDhlMw",
    "expires_in": 86400,
    "token_type": "bearer",
    "scope": "Sahip olduğunuz izinler burada görüntülünecektir.",
    "refresh_token": "NjczY2E2NzZiNDJjZWI5NTE2YjZhNTlhYTJmOTQ5MjljN2MwNmUxM2MzODc5YTE4OGVjMDdlYTBiMzY1MWI1Mw"
    }
     */
    public async Task OnGet(int mgzId, string mgzUri, string client_id, string client_secret, string code)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"grant_type", "authorization_code"},
            {"client_id",   client_id },
            {"client_secret", client_secret },
            {"code", code },
            {"redirect_uri", $"http://api.linkno.net/auth/{mgzId}" }, 
        };

        string uri = QueryHelpers.AddQueryString($"{mgzUri}/oauth/v2/token", queryParams!);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var httpClient = _httpClient.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            Token = await JsonSerializer.DeserializeAsync<TokenModel>(contentStream);

            // Save Token
            // Log
        }
        else
        {
            // Log
        }
    }
}
