using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetNewToken
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IDataAccess db;

    public GetNewToken(IHttpClientFactory httpClientFactory, IDataAccess dataAccess)
    {
        _httpClient = httpClientFactory!;
        db = dataAccess;
    }

    public async Task OnGetIfExpired(int mgzId)
    {
        string sql = $"select MTid, Expires_At, Uri, Client_Id, Client_Secret, Refresh_Token from MT where MTid = @MTid";
        var mgz = await db.LoadRec<MgzSecrets, dynamic>(sql, new { MTid = mgzId });
        
        if(mgz.Expires_At > DateTime.Now)   // Expire olmamis
            return;

        var queryParams = new Dictionary<string, string>()
        {
            {"grant_type", "refresh_token"},
            {"client_id",   mgz.Client_Id },
            {"client_secret", mgz.Client_Secret },
            {"refresh_token", mgz.Refresh_Token },
        };

        string uri = QueryHelpers.AddQueryString($"{mgz.Uri}/oauth/v2/token", queryParams!);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var httpClient = _httpClient.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var Token = await JsonSerializer.DeserializeAsync<TokenModel>(contentStream);

            string usql = "update MT set access_token = @Access_Token, refresh_token = @Refresh_Token where MTid = @MTid";
            if (await db.SaveData(usql, Token))
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
