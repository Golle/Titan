using System.IO.Compression;
using System.Text.Json;
using Titan.Core.Logging;
using Titan.Tools.Core.Common;

namespace Titan.Tools.Packager.PreReqs;

internal class DownloadDXCCompiler
{
    private const string GithubAcceptHeaderValue = "application/vnd.github.v3+json";
    private const string BaseUrl = "https://api.github.com";
    private const string Owner = "microsoft";
    private const string Repo = "DirectXShaderCompiler";
    private const string DXXGithubPath = $"{BaseUrl}/repos/{Owner}/{Repo}/releases";

    private static readonly HttpClient Client = new();

    public static async Task<Result<string>> Download()
    {
        Client.DefaultRequestHeaders.Accept.ParseAdd(GithubAcceptHeaderValue);
        var request = new HttpRequestMessage(HttpMethod.Get, DXXGithubPath)
        {
            Headers =
            {
                { "user-agent", "titan-game-engine" },
                { "accepts", GithubAcceptHeaderValue }
            }
        };

        var result = await Client.SendAsync(request);
        if (!result.IsSuccessStatusCode)
        {
            return Result<string>.Fail($"Getting version from Github failed with code: {result.StatusCode}");
        }

        var resultStream = await result.Content.ReadAsStreamAsync();
        var releases = await Json.DeserializeAsync<GithubRelease[]>(resultStream, new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = false
        });

        if (releases?.Length is null)
        {
            return Result<string>.Fail("Failed to parse the versions from github");
        }
        var latestRelease = releases
            .Where(r => !r.PreRelease) // Ignore pre-releases
            .MaxBy(r => r.PublishedAt);
        if (latestRelease?.Assets == null)
        {
            return Result<string>.Fail("Failed to find any releases.");
        }

        //NOTE(Jens): check with current version
        //if (latestRelease.PublishedAt == context.CurrentVersion?.PublishedAt && latestRelease.Name == context.CurrentVersion?.Name)
        //{
        //    Logger.Info($"We're already on the latest version, aborting. ({latestRelease.Name} - {latestRelease.PublishedAt})");
        //    return context;
        //}

        var asset = latestRelease.Assets.FirstOrDefault(a => a.Name.Contains("dxc", StringComparison.OrdinalIgnoreCase));
        if (asset == null)
        {
            return Result<string>.Fail($"Failed to find a DXC asset. Available: {string.Join(',', latestRelease.Assets.Select(a => a.Name))}");
        }

        var tempPath = Path.GetTempPath();
        var libPath = Path.Combine(tempPath, "dxc");
        Logger.Info<DownloadDXCCompiler>($"Using lib path {libPath}");
        if (Directory.Exists(libPath))
        {
            Logger.Info<DownloadDXCCompiler>("delete old directory");
            Directory.Delete(libPath, true);
        }
        Directory.CreateDirectory(libPath);

        await using var assetStream = await Client.GetStreamAsync(asset.BrowserDownloadUrl);
        var zipFileName = Path.Combine(libPath, $"{asset.Name.Replace(' ', '_')}");
        {
            Logger.Info<DownloadDXCCompiler>($"Creating zip file {zipFileName}");
            await using var file = File.OpenWrite(zipFileName);
            file.SetLength(0);
            await assetStream.CopyToAsync(file);
        }
        ZipFile.ExtractToDirectory(zipFileName, libPath, true);


        return Result<string>.Success(Path.Combine(libPath, "bin", "x64"));
    }


}
internal record GithubRelease(string Name, string TagName, DateTime PublishedAt, bool PreRelease, string Url, GithubAsset[]? Assets);
internal record GithubAsset(string Name, string ContentType, string BrowserDownloadUrl, long Size);

internal class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        Span<char> buffer = stackalloc char[name.Length * 2];
        var charCount = 0;

        for (var i = 0; i < name.Length; i++)
        {
            var character = name[i];
            if (i != 0 && char.IsUpper(character))
            {
                buffer[charCount++] = '_';
            }

            buffer[charCount++] = char.ToLower(character);
        }

        return new string(buffer[..charCount]);
    }
}
