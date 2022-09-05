using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetOrders
{
    private readonly IHttpClientFactory _httpClient;

    public GetOrders(IHttpClientFactory httpClientFactory) 
    { 
        _httpClient = httpClientFactory; 
    }

    public IEnumerable<OrderModel>? Orders { get; set; }

    public async Task OnGet(string accessToken, int mgzId, string mgzUri, DateTime startUpdatedAt, DateTime endUpdatedAt)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"startUpdatedAt", startUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
            {"endUpdatedAt",   endUpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
        };

        string uri = QueryHelpers.AddQueryString($"{mgzUri}/api/orders", queryParams);


        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
        {
            Headers =
            {
                { HeaderNames.Authorization, $"Bearer {accessToken}" },
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
