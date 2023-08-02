
using System.Text.Json;

namespace Tests.Integration.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage responseMessage)
    {
        var content = await responseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content)!;
    }
}