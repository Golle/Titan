Texture2D tex : register(t0);

SamplerState splr;

struct PS_INPUT
{
    float3 Normal: Normal;
    float2 Texture: Texture;
    float4 Position: SV_Position;
};

float4 main(PS_INPUT input) : SV_TARGET
{
    // return float4(1.0,1.0,1.0,1.0);
    return tex.Sample(splr, input.Texture);
 	// return input.Color;
}
