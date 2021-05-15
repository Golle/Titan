using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Titan.Core.IO;
using Titan.Core.Threading;
using Titan.IOC;

namespace Titan.Assets
{
    public class TestClassForASsets
    {

        public void Run(IContainer container)
        {
            
            
            //IOWorkerPool.Initialize();

            var manager = new AssetManager(container.GetInstance<FileSystem>());


            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
                manager.Load<int>(@"textures\lion.png");
            });
            
        }
    }
}
