using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Titan.GraphicsV2.Rendering
{
    public enum FrameBufferTypes
    {
        None
    }


    public class RenderPassBuilder
    {



        public RenderPassBuilder WithRenderTarget()
        {
            return this;
        }



        public object Build()
        {
            return null;
        }
    }

    
}
