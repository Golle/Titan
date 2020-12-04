using System;
using System.Diagnostics;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.IOC;

namespace Titan
{
    public class ECSTestClass
    {
        public void Run(IContainer container)
        {

            var world = new WorldBuilder(container, maxEntities: 10_000)
                .WithComponent<Transform3D>()
                .WithComponent<Transform2D>()
                .Build();

            var entites = new Entity[100];
            for (var i = 0; i < entites.Length; ++i)
            {
                entites[i] = world.CreateEntity();
                if (i % 2 == 0)
                {
                    entites[i].AddComponent<Transform3D>();
                }
                else
                {
                    entites[i].AddComponent(new Transform3D());
                }
            }


        }
    }
}
