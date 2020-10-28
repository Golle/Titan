cbuffer PerFrameBuffer : register(b0)
{
    matrix View;
    matrix ViewProjection;
};

cbuffer PerObjectBuffer : register(b1)
{
    matrix World;
};

#ifdef NORMAL_MAP
    struct VS_INPUT 
    {
        float3 Position: POSITION;
        float2 Texture: Texture;
    };
    struct VS_OUTPUT 
    {
        float2 Texture: Texture;
        float4 Position: SV_Position;
    };

#else
    struct VS_INPUT 
    {
        float3 Position: POSITION;
        float3 Normal: Normal;
        float2 Texture: Texture;
    };

    struct VS_OUTPUT 
    {
        float3 Normal: Normal;
        float2 Texture: Texture;
        float4 Position: SV_Position;
    };
#endif


VS_OUTPUT main(VS_INPUT input) 
{
    VS_OUTPUT output;

    float4 position = mul(float4(input.Position, 1.0f), World);
    output.Position = mul(position, ViewProjection);
    
    output.Texture = input.Texture;

#ifndef NORMAL_MAP
    output.Normal = mul(input.Normal, (float3x3) World);
#endif

    return output;
}
