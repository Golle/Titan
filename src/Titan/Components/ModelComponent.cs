using System.Runtime.InteropServices;
using Titan.Assets.Models;
using Titan.Core;

namespace Titan.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ModelComponent
    {
        public Handle<Model> Handle;
    }
}