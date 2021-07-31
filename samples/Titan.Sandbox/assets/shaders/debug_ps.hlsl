Texture2D tex : register(t0);

SamplerState splr;

float4 main(float2 textureCoords: Texture) : SV_TARGET
{
    float4 color = tex.Sample(splr, textureCoords);
    return color;
    // return float4(color.xyz, 0.0f);
}
