using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.Components;

[StructLayout(LayoutKind.Sequential)]
public struct BoxColliderComponent
{
    public const int MaxOverlappingEntities = 5; //TODO: make this configurable with a Source Generator if possible?
    
    public Margins Margins;
    public uint ColliderMask;

    
    internal int OverlappingEntites;
    internal unsafe fixed uint Entities[MaxOverlappingEntities];

    internal Vector2 BottomLeft;
    internal Vector2 TopRight;
}
