using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Meshes;

namespace Titan.Graphics.Pipeline.Graph
{

    public class RenderGraphTester
    {

        public void Run()
        {

        }

        //public void RunOldSample()
        //{
        //    var gbufferRenderPass = new RenderPass("GBuffer")
        //        .AddTarget(new PassTarget("Albedo", DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM))
        //        .AddTarget(new PassTarget("Normals", DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM))
        //        .SetDepthStencil(new DepthStencil("depth", DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS))
        //        ;

        //    var lightingPass = new RenderPass("DeferredShading")
        //        .AddTarget(new PassTarget("Lights", DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM))
        //        .AddSource("Albedo")
        //        .AddSource("Normals")
        //        ;

        //    var backbufferPass = new RenderPass("Backbuffer")
        //        .AddSource("Lights")
        //        .AddTarget(new PassTarget("$Backbuffer", DXGI_FORMAT.DXGI_FORMAT_UNKNOWN));


        //    var renderGraph = new RenderGraphBuilder()
        //        .AddPass(gbufferRenderPass)
        //        .AddPass(lightingPass)
        //        .AddPass(backbufferPass)
        //        .Compile();


        //}
    }


    public record RenderBuffer(RenderTargetView RenderTargetView, ShaderResourceView ShaderResourceView, Texture2D Texture) : IDisposable
    {
        public void Dispose()
        {
            RenderTargetView?.Dispose();
            ShaderResourceView?.Dispose();
            Texture?.Dispose();
        }
    }

    public struct Renderable
    {
        public Mesh Mesh;
        public Matrix4x4 World;
    }

    public interface IRenderPass : IDisposable
    {

    }


    #region old builder code
    //internal record PassTarget(string Name, DXGI_FORMAT Format, float SizeX = 1f, float SizeY = 1f);
    //internal record DepthStencil(string Name, DXGI_FORMAT Format);

    //internal class RenderPass : IRenderPass
    //{
    //    public string Name { get; }
    //    private readonly IList<string> _sources = new List<string>();
    //    private readonly IList<PassTarget> _targets = new List<PassTarget>();
    //    public DepthStencil DepthStencil { get; private set; }

    //    public RenderPass(string name)
    //    {
    //        Name = name;
    //    }

    //    public RenderPass AddSource(string name)
    //    {
    //        _sources.Add(name);
    //        return this;
    //    }

    //    public RenderPass AddTarget(PassTarget target)
    //    {
    //        _targets.Add(target);
    //        return this;
    //    }

    //    public RenderPass SetDepthStencil(DepthStencil depthStencil)
    //    {
    //        DepthStencil = depthStencil;
    //        return this;
    //    }
    //}


    //internal interface IRenderPass
    //{
    //    string Name { get; }

    //}


    //internal class RenderGraphBuilder
    //{
    //    private readonly IList<IRenderPass> _renderPasses = new List<IRenderPass>();
    //    public RenderGraphBuilder AddPass(IRenderPass renderPass)
    //    {
    //        Debug.Assert(_renderPasses.Any(r => r.Name == renderPass.Name) == false, $"RenderPass {renderPass.Name} has already been added.");
    //        _renderPasses.Add(renderPass);
    //        return this;
    //    }



    //    public IRenderGraph Compile()
    //    {
    //        var renderGraph = new RenderGraph();



    //        return renderGraph;
    //    }

    //}

    //internal interface IRenderGraph
    //{

    //}

    //internal class RenderGraph : IRenderGraph
    //{
    //    public RenderGraph()
    //    {

    //    }
    //}
#endregion
}
