Texture2D tex : register(t0);
SamplerState splr : register(s0);

#ifdef NORMAL_MAP 
    // normal texture file 
    Texture2D normalMap : register(t1);
    SamplerState normalSplr : register(s1);
    
    struct PS_INPUT
    {
        float3 Normal: Normal;
        float2 Texture: Texture;
        float4 Position: Position;
    };

    float4 GetNormal(PS_INPUT input) 
    {
        // float4 bumpMap = normalMap.Sample(normalSplr, input.Texture);
        // bumpMap = (bumpMap * 2.0f) - 1.0f;

        // float4 result = float4((bumpMap.z * input.Normal), 1.0f);
        // // // bumpNormal = (bumpMap.x * input.tangent) + (bumpMap.y * input.binormal) + (bumpMap.z * input.Normal);
        
        // return normalize(result);

        // return normalMap.Sample(normalSplr, input.Texture);
        return float4(input.Normal, 1.0);
    }
#else
    // normal per vertex 
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
#endif

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
    output.Albedo = tex.Sample(splr, input.Texture);
    output.Normal = GetNormal(input);

    return output;
}

