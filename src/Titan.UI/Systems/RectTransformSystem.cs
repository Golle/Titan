using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.Graphics;
using Titan.Graphics.Windows;
using Titan.UI.Common;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class RectTransformSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<RectTransform> _transform;

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>());

            _transform = GetMutable<RectTransform>();
        }
        
        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);
                
                transform.AbsolutePivot = new Vector2(transform.Size.Width * transform.Pivot.X, transform.Size.Height * transform.Pivot.Y);
                if (EntityManager.TryGetParent(entity, out var parent) && _transform.Contains(parent))
                {
                    ref readonly var parentTransform = ref _transform.Get(parent);
                    
                    transform.AbsolutePosition = CalculateNewPosition(parentTransform.AbsolutePosition, parentTransform.Size, transform);
                    transform.AbsoluteZIndex = transform.ZIndex + parentTransform.AbsoluteZIndex + 1;

                    //TODO: not sure how we should do this. use constraints maybe?
                    if (transform.Size.Height == 0 && transform.Size.Width == 0)
                    {
                        transform.Size = parentTransform.Size;
                    }
                }
                else
                {
                    var windowSize = new Size(Window.Width, Window.Height);
                    transform.AbsolutePosition = CalculateNewPosition(Vector2.Zero, windowSize, transform);
                    transform.AbsoluteZIndex = transform.ZIndex;
                }
            }


            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            static Vector2 CalculateNewPosition(in Vector2 parentPosition, in Size parentSize, in RectTransform transform)
            {
                ref readonly var offset = ref transform.Offset;
                ref readonly var pivot = ref transform.AbsolutePivot;

                var anchorPoint = transform.AnchorPoint;

                Unsafe.SkipInit<Vector2>(out var position);
                position.X = (anchorPoint & AnchorPoint.HorizontalMask) switch
                {
                    AnchorPoint.Right => parentPosition.X + parentSize.Width - pivot.X + offset.X,
                    AnchorPoint.Center => parentPosition.X + parentSize.Width / 2f - pivot.X + offset.X,
                    _ => parentPosition.X + offset.X - pivot.X // This is Left as well
                };

                position.Y = (anchorPoint & AnchorPoint.VerticalMask) switch
                {
                    AnchorPoint.Top => parentPosition.Y + parentSize.Height - pivot.Y + offset.Y, 
                    AnchorPoint.Middle => parentPosition.Y + parentSize.Height / 2f - pivot.Y + offset.Y,
                    _ => parentPosition.Y + offset.Y - pivot.Y // This is Bottom as well
                };
                return position;
            }
        }
    }
}
