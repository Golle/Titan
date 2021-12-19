using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.UI.Components;

namespace Titan.UI.Animation;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AnimationState
{
    public float Time;
    public float CurrentTime;
    public bool Loop;
    public bool Mirror;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct AnimateTranslation
{
    public Vector2 Start;
    public Vector2 End;
    public AnimationState State;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct AnimateScale
{
    public Vector3 Start;
    public Vector3 End;
    public AnimationState State;
}


public class AnimateTranslationSystem : EntitySystem
{
    private EntityFilter _filter;
    private MutableStorage<RectTransform> _transform;
    private MutableStorage<AnimateTranslation> _animation;

    protected override void Init(IServiceCollection services)
    {
        _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<AnimateTranslation>());

        _transform = GetMutable<RectTransform>();
        _animation = GetMutable<AnimateTranslation>();
    }

    protected override void OnUpdate(in Timestep timestep)
    {
        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref var animation = ref _animation.Get(entity);
            ref var transform = ref _transform.Get(entity);
                
            animation.State.CurrentTime += timestep.Seconds;
                
            var amount = animation.State.CurrentTime / animation.State.Time;
            if (amount <= 1.0f)
            {
                transform.Offset = Vector2.Lerp(animation.Start, animation.End, amount);
            }
            else
            {
                (animation.End, animation.Start) = (animation.Start, animation.End);
                animation.State.CurrentTime = 0f;
            }
        }
    }
}
