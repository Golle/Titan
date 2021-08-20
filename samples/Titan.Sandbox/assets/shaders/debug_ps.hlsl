Texture2D tex : register(t0);

SamplerState splr;

float4 main(float2 textureCoords: Texture) : SV_TARGET
{
    float4 color = tex.Sample(splr, textureCoords);
    
    // Discard the black background
    float c = color.r + color.g + color.b;
    if(c == 0){
        discard;
    }
    return color;
}
