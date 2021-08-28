
Texture2D tex : register(t0);

SamplerState splr : register(s0);

struct PS_INPUT {
    float2 Texture: Texture;
    float4 Color: Color;
};

float4 main(PS_INPUT input) : SV_TARGET {
    
    float4 color = tex.Sample(splr, input.Texture) * input.Color;
    return color;
    
    // Used for debugging.
    // if(color.a > 0.0f){
    //     return color;
    // }
    // return float4(1,0,0,1);
}
