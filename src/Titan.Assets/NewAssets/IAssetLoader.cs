using System;
using System.IO;
using Titan.Core;
using Titan.Core.Services;

namespace Titan.Assets.NewAssets;

public interface IAssetLoader<T> where T : unmanaged
{
    static abstract Handle<T> Load(ReadOnlySpan<byte> data);
    static abstract void Unload(Handle<T> handle);
}
