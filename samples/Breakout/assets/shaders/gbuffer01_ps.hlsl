Texture2D tex : register(t0);
SamplerState splr : register(s0);

cbuffer Material : register(b1) {
    float4 DiffuseColor;
    float4 AmbientColor;
};

struct PS_INPUT
{
    float3 Normal: Normal;
    float2 Texture: Texture;
    float4 Position: Position;
};

float4 GetNormal(PS_INPUT input) 
{
    return float4(input.Normal, 1.0);
}

struct PS_OUTPUT 
{
    float4 Position: Color0;
    float4 Albedo: Color1;
    float4 Normal: Color2;
};

PS_OUTPUT main(PS_INPUT input) : SV_TARGET
{
    PS_OUTPUT output;
    output.Position = input.Position;
    output.Albedo = DiffuseColor;
    // output.Albedo = tex.Sample(splr, input.Texture);
    output.Normal = GetNormal(input);

    return output;
}

