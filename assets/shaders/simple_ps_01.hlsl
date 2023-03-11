// Texture2D tex : register(t0);

// SamplerState splr;

float4 main(float2 textureCoords: Texture) : SV_TARGET
{
    float4 color = float4(1,0,1,0); // tex.Sample(splr, textureCoords);
    
    return color;
}
