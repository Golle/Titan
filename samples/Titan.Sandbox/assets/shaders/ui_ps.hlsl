
Texture2D tex : register(t0);

SamplerState splr : register(s0);

struct PS_INPUT {
    float2 Texture: Texture;
};


float4 main(PS_INPUT input) : SV_TARGET {
    return tex.Sample(splr, input.Texture);
}
