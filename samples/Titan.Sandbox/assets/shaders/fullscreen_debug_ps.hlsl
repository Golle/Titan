Texture2D tex : register(t0);
Texture2D ui : register(t1);
Texture2D debug : register(t2);

SamplerState splr;

float4 main(float2 textureCoords: Texture) : SV_TARGET
{
    float4 color = tex.Sample(splr, textureCoords);
    float4 uiColor = ui.Sample(splr, textureCoords);
    float4 debugColor = debug.Sample(splr, textureCoords);
    
    return color + uiColor + debugColor;
}
