using System;
using Titan;
using Titan.Windows;

using var window = Bootstrapper
    .Container
    .GetInstance<IWindowFactory>()
    .Create(1920, 1080, "Donkey box #2!");
    
while (window.Update())
{
    GC.Collect();
}
