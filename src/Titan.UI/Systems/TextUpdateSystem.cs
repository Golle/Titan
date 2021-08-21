using System;
using Titan.ECS.Systems;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{
    internal class TextUpdateSystem : EntitySystem
    {
        private readonly TextManager _textManager;
        private EntityFilter _filter;
        private MutableStorage<TextComponent> _text;

        public TextUpdateSystem(TextManager textManager)
        {
            _textManager = textManager;
        }
        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<TextComponent>());
            _text = GetMutable<TextComponent>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                var textHandle = _text.Get(entity).Text;
                ref readonly var textBlock = ref _textManager.Access(textHandle);
                if (textBlock.IsDirty)
                {
                    _textManager.ReCalculate(textHandle);
                }
            }
        }
    }
}
