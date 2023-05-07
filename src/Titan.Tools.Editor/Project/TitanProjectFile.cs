using System.Text.Json;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Core;

namespace Titan.Tools.Editor.Project;

internal class TitanProjectFile : ITitanProjectFile
{
    private readonly IFileSystem _fileSystem;
    public TitanProjectFile(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<Result> Save(TitanProject project, string path)
    {
        try
        {
            var json = JsonSerializer.Serialize(project, TitanProjectJsonContext.Default.TitanProject);
            await _fileSystem.WriteText(path, json);
        }
        catch (Exception e)
        {
            return Result.Fail($"Failed with a {e.GetType()} - {e.Message}");
        }
        return Result.Ok();
    }

    public async Task<Result<TitanProject>> Read(string path)
    {
        if (!_fileSystem.Exists(path))
        {
            return Result.Fail<TitanProject>($"The project file at path {path} does not exist.");
        }

        var contents = await _fileSystem.ReadBytes(path);
        if (contents.Length == 0)
        {
            return Result.Fail<TitanProject>($"The length of the contents of file {path} is 0");
        }

        try
        {
            var project = JsonSerializer.Deserialize(contents, TitanProjectJsonContext.Default.TitanProject);
            return project != null ?
                Result.Ok(project) :
                Result.Fail<TitanProject>($"Failed to deserialize project at path {path}");
        }
        catch (Exception e)
        {
            return Result.Fail<TitanProject>($"Failed to deserialize project at path {path} with a {e.GetType().Name} - {e.Message}");
        }
    }
}
