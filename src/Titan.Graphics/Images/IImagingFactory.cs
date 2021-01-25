using System;

namespace Titan.Graphics.Images
{
    public interface IImagingFactory : IDisposable
    {
        Image LoadImageFromFile(string filename);
    }
}
