using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetFirstToken
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IDataAccess db;

    public GetFirstToken(IHttpClientFactory httpClientFactory, IDataAccess dataAccess)
    {
        _httpClient = httpClientFactory!;
        db = dataAccess;
    }

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

            var Token = await JsonSerializer.DeserializeAsync<TokenModel>(contentStream);

            Token.MTid = mgzId;
            string usql = "update MT set access_token = @Access_Token, refresh_token = @Refresh_Token where MTid = @MTid";
            if(await db.SaveData(usql, Token))
            {
                Console.WriteLine("Saved");
            }
            else
            {
                Console.WriteLine("Basarisiz");
            }
        }
        else
        {
            // Log
        }
    }
}
