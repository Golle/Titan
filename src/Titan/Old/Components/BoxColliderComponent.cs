using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.Old.Components;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ReadOnlySpan<uint> GetOverlappingEntites()
    {
        fixed (uint* pEntites = Entities)
        {
            return new(pEntites, OverlappingEntites);
        }
    }
}
