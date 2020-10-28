Texture2D tex : register(t0);
SamplerState splr : register(s0);

#ifdef NORMAL_MAP 
    // normal texture file 
    Texture2D normalMap : register(t1);
    SamplerState normalSplr : register(s1);
    
    struct PS_INPUT
    {
         float2 Texture: Texture;
    };

    float4 GetNormal(PS_INPUT input) 
    {
        return normalMap.Sample(normalSplr, input.Texture);
    }
#else
    // normal per vertex 
    struct PS_INPUT
    {
        float3 Normal: Normal;
        float2 Texture: Texture;
    };
    float4 GetNormal(PS_INPUT input) 
    {
        return float4(input.Normal, 1.0);
    }
#endif

struct PS_OUTPUT 
{
    float4 Albedo: Color0;
    float4 Normal: Color1;
};

PS_OUTPUT main(PS_INPUT input) : SV_TARGET
{
    PS_OUTPUT output;
    output.Albedo = tex.Sample(splr, input.Texture);
    output.Normal = GetNormal(input);

    return output;
}

