using Titan.Components;
using Titan.ECS.Systems;
using Titan.ECS.TheNew;

namespace Titan.Systems;

public class Transform3DSystem : EntitySystem_
{
    private MutableStorage<Transform3DComponent> _transform;

    protected override void OnInit()
    {
        _transform = GetMutable<Transform3DComponent>();
    }

    protected override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }
}
