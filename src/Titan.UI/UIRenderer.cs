using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titan.Graphics;
using Titan.Graphics.D3D11;

namespace Titan.UI
{

    /*
    Requirements

    Get unsorted list of renderables that are "active/enabled"
    Sort by Z-index (hiearchy?)
    Sort by TextureHandle (most cases only a single texture)


    */
    public class UIRenderQueue
    {
        
    }



    class UIRenderer : IRenderer
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Render(Context context)
        {
            throw new NotImplementedException();
        }
    }
}
