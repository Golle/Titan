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
        float3 Normal: Normal;
        float2 Texture: Texture;
        float3 Tangent: Tangent;
        float3 BiNormal: BiNormal;
    };
    struct VS_OUTPUT 
    {
        float3 Normal: Normal;
        float2 Texture: Texture;
        float4 WorldPosition: Position;
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
        float4 WorldPosition: Position;
        float4 Position: SV_Position;
    };
#endif


VS_OUTPUT main(VS_INPUT input) 
{
    VS_OUTPUT output;

    output.WorldPosition = mul(float4(input.Position, 1.0f), World);
    output.Position = mul(output.WorldPosition, ViewProjection);
    output.Texture = input.Texture;
    output.Normal = mul(input.Normal, (float3x3) World);

    return output;
}
