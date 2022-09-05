using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetFirstToken
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IDataAccess db;
    public TokenModel Token { get; set; }

    public GetFirstToken(IHttpClientFactory httpClientFactory, IDataAccess dataAccess)
    {
        _httpClient = httpClientFactory!;
        db = dataAccess;
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
    public async Task OnGet(int mgzId, string code)
    {
        string sql = $"select MTid, Uri, Client_Id, Client_Secret from MT where MTid = @MTid";
        var mgz = await db.LoadRec<MgzSecrets, dynamic>(sql, new { MTid = mgzId });

        var queryParams = new Dictionary<string, string>()
        {
            {"grant_type", "authorization_code"},
            {"client_id",   mgz.Client_Id },
            {"client_secret", mgz.Client_Secret },
            {"code", code },
            {"redirect_uri", $"http://api.linkno.net/auth/{mgzId}" }, 
        };

        string uri = QueryHelpers.AddQueryString($"{mgz.Uri}/oauth/v2/token", queryParams!);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var httpClient = _httpClient.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            Token = await JsonSerializer.DeserializeAsync<TokenModel>(contentStream);

            string usql = "update MT set access_token = @Access_Token, refresh_token = @Refresh_Token where MTid = @MTid";
            if(await db.SaveData(usql, Token))
            {
                // Basarili
            }
            else
            {
                // Basarisiz
            }
        }
        else
        {
            // Log
        }
    }
}
