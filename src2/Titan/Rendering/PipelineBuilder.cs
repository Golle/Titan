using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Shaders;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Rendering
{
    internal class PipelineBuilder
    {
        private Handle<Asset> _gBufferHandle;

        private readonly AssetsManager _assetsManager;
        private Handle<Asset> _fullscreenHandle;
        private GeometryRenderer _geometryRenderer;
        private BackbufferRenderer _backbufferRenderer;

        public PipelineBuilder(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        public void LoadResources()
        {
            _gBufferHandle = _assetsManager.Load("shaders/gbuffer");
            _fullscreenHandle = _assetsManager.Load("shaders/fullscreen");

            _geometryRenderer = new GeometryRenderer();
            _backbufferRenderer = new BackbufferRenderer();
        }

        public bool IsReady()
        {
            return _assetsManager.IsLoaded(_gBufferHandle) &&
                   _assetsManager.IsLoaded(_fullscreenHandle);
        }

        public Pipeline[] Create()
        {
            Debug.Assert(GraphicsDevice.IsInitialized, $"{nameof(GraphicsDevice)} must be initialized before the {nameof(GraphicsSystem)} is created.");

            var swapchain = GraphicsDevice.SwapChain;

            // Create the framebuffers
            var gBufferAlbedo = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });
            var gBufferNormals = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });
            var gBufferProperties = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });

            var gbufferShaders = _assetsManager.GetAssetHandle<ShaderProgram>(_gBufferHandle);

            var fullscreenSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
            {
                Filter = TextureFilter.MinMagMipPoint,
                AddressAll = TextureAddressMode.Wrap
            });

            var gBuffer = new Pipeline
            {
                RenderTargets = new[] {gBufferAlbedo, gBufferNormals, gBufferProperties},
                PixelShader = gbufferShaders.PixelShader,
                VertexShader =  gbufferShaders.VertexShader,
                ClearColor = Color.Magenta,
                ClearRenderTargets = true,
                Renderer = _geometryRenderer
            };

            var backbufferRenderTarget = GraphicsDevice.TextureManager.CreateBackbufferRenderTarget();

            //var backbufferColor = GraphicsDevice.TextureManager.Create(new TextureCreation
            //{
            //    Format = TextureFormats.RGBA32F,
            //    Width = swapchain.Width,
            //    Height = swapchain.Height,
            //    Binding = TextureBindFlags.FrameBuffer
            //});


            var fullscreenShader = _assetsManager.GetAssetHandle<ShaderProgram>(_fullscreenHandle);
            var backbuffer = new Pipeline
            {
                ClearDepthStencil = false,
                ClearRenderTargets = true,
                ClearColor = Color.Green,
                RenderTargets = new[] {backbufferRenderTarget},
                PixelShader = fullscreenShader.PixelShader,
                VertexShader = fullscreenShader.VertexShader,
                PixelShaderResources = new[] {gBufferAlbedo},
                PixelShaderSamplers = new []{fullscreenSampler},
                Renderer = _backbufferRenderer
            };

            return new[] {gBuffer, backbuffer};
        }
    }
}
