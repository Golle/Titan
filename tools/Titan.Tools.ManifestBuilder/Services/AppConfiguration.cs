using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Titan.Tools.ManifestBuilder.Services;

internal record AppConfig(string ProjectBasePath);

internal interface IAppConfiguration
{
    Task CreateOrInit(string basePath);
    Task<AppConfig> GetConfig();
}

/// <summary>
/// This class will take care of any configuration (right now there's no config except for the base path)
/// </summary>
internal class AppConfiguration : IAppConfiguration
{
    private string? _basePath;
    public Task CreateOrInit(string basePath)
    {
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
        _basePath = basePath;
        return Task.CompletedTask;
    }

    public Task<AppConfig> GetConfig()
    {
        if (string.IsNullOrWhiteSpace(_basePath))
        {
            throw new InvalidOperationException("Base path has not been set, this is an invalid state.");
        }
        return Task.FromResult(new AppConfig(_basePath!));
    }
}
