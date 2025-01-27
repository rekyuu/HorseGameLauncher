using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HorseGameLauncher.Config;
using Serilog;

namespace HorseGameLauncher.Utility;

internal static class HttpUtility
{
    private static readonly HttpClient Client;

    static HttpUtility()
    {
        Client = new HttpClient();

        string userAgentName = HorseGameLauncherConfig.ApplicationName
            .Replace(" ", "")
            .Replace("-", "");

        Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgentName, HorseGameLauncherConfig.ApplicationVersion));
        Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"({RuntimeInformation.OSDescription}: {RuntimeInformation.OSArchitecture})"));
        Client.Timeout = TimeSpan.FromMinutes(5);
    }

    internal static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return await Client.SendAsync(request);
    }

    internal static async Task DownloadFileAsync(string uri, string destinationPath)
    {
        try
        {
            HttpResponseMessage response = await Client.GetAsync(uri);

            await using FileStream stream = File.Create(destinationPath);
            await response.Content.CopyToAsync(stream);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to download {Uri}", uri);
        }
    }
}