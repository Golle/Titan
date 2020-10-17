Texture2D tex : register(t0);

SamplerState splr;

struct PS_INPUT
{
    float4 Position: SV_POSITION;
    float2 Texture: Texture;
    float4 Color: Color;
};

float4 main(PS_INPUT input) : SV_TARGET
{
    return tex.Sample(splr, input.Texture) * input.Color;
 	// return input.Color;
}
