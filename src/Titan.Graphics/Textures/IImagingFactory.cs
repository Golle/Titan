using System;

namespace Titan.Graphics.Textures
{
    internal interface IImagingFactory : IDisposable
    {
        IImageDecoder CreateDecoderFromFilename(string filename);
    }
}
