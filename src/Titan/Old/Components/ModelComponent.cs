using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.Loaders.Models;

namespace Titan.Old.Components;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ModelComponent
{
    public Handle<Model> Handle;
}
