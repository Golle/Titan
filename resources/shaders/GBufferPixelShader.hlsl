Texture2D tex : register(t0);
Texture2D depth : register(t1);

SamplerState splr;

struct PS_INPUT
{
    float3 Normal: Normal;
    float2 Texture: Texture;
    float4 Position: SV_Position;
};

struct PS_OUTPUT 
{
    float4 Albedo: Color0;
    float4 Normal: Color1;
    float4 Depth: Color2;
};

PS_OUTPUT main(PS_INPUT input) : SV_TARGET
{
    PS_OUTPUT output;
    output.Albedo = tex.Sample(splr, input.Texture);
    output.Normal = float4(input.Normal, 1.0);
    output.Depth = depth.Sample(splr, input.Texture);

    return output;
}
