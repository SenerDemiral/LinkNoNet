using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetOrders
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IDataAccess db;
    private readonly GetNewToken _getNewToken;

    public GetOrders(IHttpClientFactory httpClientFactory, IDataAccess dataAccess, GetNewToken getNewToken) 
    { 
        _httpClient = httpClientFactory; 
        db = dataAccess;
        _getNewToken = getNewToken;
    }

    public IEnumerable<OrderModel>? Orders { get; set; }

    public async Task OnGet(int mgzId)
    {
        await _getNewToken.OnGetIfExpired(mgzId);    // Expired ise NewToken alip db ye yazar.

        string sql = "select Last_EXD, Uri, Access_Token from MT where MTid = @MTid";
        var mgz = await db.LoadRec<MgzSecrets, dynamic>(sql, new { MTid = mgzId });

        DateTime startUpdatedAt = mgz.Last_EXD;
        DateTime endUpdatedAt = DateTime.Now.AddMinutes(-5);

        string usql = "update MT set last_EXD = @Last_EXD where MTid = @MTid";
        await db.SaveData(usql, new { last_EXD = endUpdatedAt, MTid = mgzId });


        var queryParams = new Dictionary<string, string>()
        {
            {"startUpdatedAt", startUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
            {"endUpdatedAt",   endUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
        };

        string uri = QueryHelpers.AddQueryString($"{mgz.Uri}/api/orders", queryParams);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
        {
            Headers =
            {
                { HeaderNames.Authorization, $"Bearer {mgz.Access_Token}" },
                { HeaderNames.ContentType, "application/json; charset=utf-8" }
            }
        };

        var httpClient = _httpClient.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            Orders = await JsonSerializer.DeserializeAsync<IEnumerable<OrderModel>>(contentStream);
        }
    }
}
