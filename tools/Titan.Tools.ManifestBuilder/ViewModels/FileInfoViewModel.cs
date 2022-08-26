using System;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class FileInfoViewModel : ViewModelBase
{
    public required string FileName { get; init; }
    public string? FilePath { get; init; }
    public DateTime? FileCreated { get; init; }
    public DateTime? FileChanged { get; init; }
    public long? Size { get; init; }
    public bool HasSize => Size != null;
    public bool IsImage => Image != null;
    public IImage? Image { get; init; }

    //NOTE(Jens): add more if we need it
    private static readonly string[] SupportedImageFileExtensions = { ".jpg", ".png", ".bmp" };
    public static FileInfoViewModel Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return new FileInfoViewModel
            {
                FileName = "n/a"
            };
        }

        var fileInfo = new FileInfo(path);
        long? size = (fileInfo.Attributes & FileAttributes.Directory) == 0 ? fileInfo.Length : null;
        var isImage = SupportedImageFileExtensions.Contains(Path.GetExtension(fileInfo.Name), StringComparer.OrdinalIgnoreCase);
        return new FileInfoViewModel
        {
            Size = size,
            FileCreated = fileInfo.CreationTime,
            FileChanged = fileInfo.LastWriteTime,
            FileName = fileInfo.Name,
            FilePath = path,
            Image =  isImage? new Bitmap(path) : null
        };

    }
    
}
