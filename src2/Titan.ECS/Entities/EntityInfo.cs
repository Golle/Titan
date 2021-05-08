using System.Runtime.InteropServices;
using Titan.ECS.Components;

namespace Titan.ECS.Entities
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct EntityInfo
    {
        public ComponentId Components;
    }
}
