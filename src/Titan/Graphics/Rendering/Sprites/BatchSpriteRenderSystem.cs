using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Graphics.D3D12.Memory;
using Titan.Platform.Win32.D3D12;
using Titan.Systems;

namespace Titan.Graphics.Rendering.Sprites;

internal struct BatchSpriteRenderSystem : ISystem
{
    private EntityQuery _query;
    private EntityQuery _cameraQuery;

    private ReadOnlyStorage<Sprite> _sprites;
    private ReadOnlyStorage<Transform2D> _transform;
    private ReadOnlyStorage<TextureComponent> _textures;
    private ReadOnlyStorage<Camera2D> _cameras;
    private ReadOnlyResource<RenderInfo> _renderInfo;

    private ObjectHandle<D3D12SpriteRenderer> _spriteRenderer;
    private ObjectHandle<D3D12CommandQueue> _commandQueue;
    private ObjectHandle<D3D12Allocator> _allocator;
    private TempArena _tempArena;


    public void Init(in SystemInitializer init)
    {
        _query = init.CreateQuery(new EntityQueryArgs().With<Sprite>().With<TextureComponent>().With<Transform2D>());
        _cameraQuery = init.CreateQuery(new EntityQueryArgs().With<Camera2D>().With<Transform2D>());
        _sprites = init.GetReadOnlyStorage<Sprite>();
        _transform = init.GetReadOnlyStorage<Transform2D>();
        _textures = init.GetReadOnlyStorage<TextureComponent>();
        _cameras = init.GetReadOnlyStorage<Camera2D>();
        _renderInfo = init.GetReadOnlyResource<RenderInfo>();
        _spriteRenderer = init.GetManagedApi<D3D12SpriteRenderer>();
        _commandQueue = init.GetManagedApi<D3D12CommandQueue>();
        _allocator = init.GetManagedApi<D3D12Allocator>();
        _tempArena = init.GetTempArena();
    }

    public unsafe void Update()
    {
        var renderInfo = _renderInfo.AsPointer();
        var pClearColor = &renderInfo->ClearColor;
        RenderCamera camera = default;
        if (_cameraQuery.HasEntities())
        {
            //NOTE(Jens): take the first camera.    
            var ent = _cameras.GetPointer(_cameraQuery[0]);
            camera.Scale = ent->Scale;
            camera.Position = ent->Position;
            camera.Size = ent->Size;
            pClearColor = &ent->ClearColor;
        }
        else
        {
            //NOTE(Jens): If there's no camera, use default size and position and set it to Window size.
            //NOTE(Jens): we should move this to some other place, we need similar thing for UI
            camera.Scale = Vector2.One;
            camera.Position = Vector2.Zero;
            camera.Size = renderInfo->WindowSize;
        }

        //NOTE(Jens): This is where we should do frustrum culling.
        var sortables = _tempArena.AllocArray<(Entity Entity, short Layer)>(_query.Count);
        var sortableCount = 0;
        {
            foreach (ref readonly var entity in _query)
            {
                ref readonly var sprite = ref _sprites.Get(entity);
                sortables[sortableCount++] = (entity, sprite.Layer);
            }

            //NOTE(Jens): This is superslow and we should not do this here.
            sortables.AsSpan().Sort(static (v1, v2) => v1.Layer.CompareTo(v2.Layer));
        }
        var renderables = _tempArena.AllocArray<SpriteInstanceData3>(_query.Count);
        var renderableCount = 0;
        foreach (var (entity, _) in sortables.AsReadOnlySpan())
        {
            ref readonly var sprite = ref _sprites.Get(entity);
            ref readonly var transform = ref _transform.Get(entity);
            ref readonly var texture = ref _textures.Get(entity);

            ref var data = ref renderables[renderableCount++];
            data.Offset = transform.WorldPosition;
            data.Scale = transform.WorldScale;
            data.Pivot = sprite.Pivot;
            data.Color = sprite.Color;
            data.DrawRect = (RectangleF)sprite.SourceRect;
            data.TextureId = texture.TextureId;
            data.TextureSize.X = texture.Width;
            data.TextureSize.Y = texture.Height;
            
            var (sin, cos) = MathF.SinCos(transform.WorldRotation);
            data.SinCosRotation.X = sin;
            data.SinCosRotation.Y = cos;
        }

        var renderer = _spriteRenderer.Value;
        var commandQueue = _commandQueue.Value;
        var allocator = _allocator.Value;

        var commandList = commandQueue.GetCommandList();
        commandList.Reset();

        var windowSize = renderInfo->WindowSize;
        // set the viewport
        {
            Unsafe.SkipInit(out D3D12_VIEWPORT viewport);
            viewport.Width = windowSize.Width;
            viewport.Height = windowSize.Height;
            viewport.MaxDepth = 1;
            viewport.MinDepth = viewport.TopLeftX = viewport.TopLeftY = 0;
            commandList.SetViewport(&viewport);
        }
        // set the scissor rect
        {
            Unsafe.SkipInit(out D3D12_RECT scissorRect);
            scissorRect.Right = windowSize.Width;
            scissorRect.Bottom = windowSize.Height;
            commandList.SetScissorRect(&scissorRect);
        }

        //NOTE(Jens): we should not render to the backbuffer, we should request a separate render target that we can use.
        ref readonly var backbuffer = ref renderInfo->CurrentBackbuffer;
        // set render target, barrier and clear it
        commandList.SetRenderTarget(backbuffer);
        commandList.Transition(backbuffer, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET);
        commandList.ClearRenderTarget(backbuffer.RTV.CPU, (float*)pClearColor);

        if (renderableCount > 0)
        {
            renderer.Begin(commandList);

            var heap = _allocator.Value.SRVDescriptorHeap._heap.Get();
            commandList.SetDescriptorHeaps(&heap);
            commandList.SetGraphicsRootDescriptorTable(0, allocator.SRVDescriptorHeap.GetGPUStart());

            //NOTE(Jens): Implement batching here, set a fixed max batch size. 
            //NOTE(Jens): How should we handle Alpha Blend? Could be sprites that does not need that.
            renderer.Render(commandList, camera, renderables[..renderableCount], alphaBlend: true);

            renderer.End(commandList);
        }
        //set the backbuffer to present, close the commandlist.
        commandList.Transition(backbuffer, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT);
        commandList.Close();
    }
}

