using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Json;

namespace RepeatingTask;

public class GetNewToken
{
    private readonly IHttpClientFactory _httpClient;
    public TokenModel Token { get; set; }

    public GetNewToken(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory!;
    }

    public async Task OnGet(int mgzId, string mgzUri, string client_id, string client_secret, string refresh_token)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"grant_type", "refresh_token"},
            {"client_id",   client_id },
            {"client_secret", client_secret },
            {"refresh_token", refresh_token },
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
