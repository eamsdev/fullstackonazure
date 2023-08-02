using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Tests.Integration.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetRouteAsync(this HttpClient httpClient, string route)
    {
        return await httpClient.GetAsync($"{httpClient.BaseAddress}{route}");
    }
    
    public static async Task<HttpResponseMessage> DeleteRouteAsync(this HttpClient httpClient, string route)
    {
        return await httpClient.DeleteAsync($"{httpClient.BaseAddress}{route}");
    }
    
    public static async Task<HttpResponseMessage> PutRouteAsJsonAsync(this HttpClient httpClient, string route, object? content)
    {
        return await httpClient.PutAsync($"{httpClient.BaseAddress}{route}", 
            new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, MediaTypeNames.Application.Json));
    }
    
    public static async Task<HttpResponseMessage> PostRouteAsJsonAsync(this HttpClient httpClient, string route, object? content = null)
    {
        return await httpClient.PostAsync($"{httpClient.BaseAddress}{route}", 
            new StringContent(content is null ? string.Empty : JsonSerializer.Serialize(content), Encoding.UTF8, MediaTypeNames.Application.Json));
    }
}