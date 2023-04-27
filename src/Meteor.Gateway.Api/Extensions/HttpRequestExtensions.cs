using System.Text;
using Microsoft.AspNetCore.Http.Extensions;

namespace Meteor.Gateway.Api.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<string> ToDebugViewAsync(this HttpRequest request, bool includeHeaders = true)
    {
        var sb = new StringBuilder();

        var url = request.GetDisplayUrl();
        sb.Append(url);
        sb.Append("\n");

        if (includeHeaders)
        {
            foreach (var (key, value) in request.Headers)
            {
                sb.Append($"{key}: {value}\n");
            }

            sb.Append("\n");
        }

        using var streamReader = new StreamReader(request.Body);
        request.Body.Position = 0;
        var body = await streamReader.ReadToEndAsync();
        request.Body.Position = 0;
        if (body.Length > 0)
        {
            sb.Append(body);
            sb.Append("\n");
        }

        return sb.ToString();
    }
}