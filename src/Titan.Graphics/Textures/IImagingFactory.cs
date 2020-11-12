using System;

namespace Titan.Graphics.Textures
{
    internal interface IImagingFactory : IDisposable
    {
        IImage LoadImageFromFile(string filename);
    }
}
