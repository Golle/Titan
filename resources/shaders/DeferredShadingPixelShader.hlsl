Texture2D albedoTexture : register(t0);
Texture2D normalTexture : register(t1);

SamplerState splr;
cbuffer LightSource : register(b0)
{
    float3 Position;
};

static const float3 DiffuseLightDirection = float3(0.0f, -0.5f, 0.5f);
static const float4 AmbientLightColor = float4(0.2f, 0.2f, 0.2f, 1.0f);
static const float4 DiffuseLightColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

static const float3 DiffuseLightColor3 = float3(1.0f, 1.0f, 1.0f);
static const float3 AmbientLightColor3 = float3(0.2f, 0.2f, 0.2f);

static const float intensity = 0.8f;



float4 main(float2 textureCoords: Texture) : SV_TARGET
{
    // return float4(1.0,1.0,1.0,1.0);
    float4 albedo = albedoTexture.Sample(splr, textureCoords);
    float4 normal = normalTexture.Sample(splr, textureCoords);
    // TODO: add light calculations
    
    
    float3 lightDirection = Position;

    float distance = length(lightDirection);
    float diffuseLightPercentage = saturate(dot(normal.xyz, normalize(lightDirection.xyz)));
    float3 diffuseLight = saturate(DiffuseLightColor3 * diffuseLightPercentage) * intensity;
    float3 totalLight = diffuseLight * intensity;
    // return float4 (1.0,1.0,1.0,1.0);s
    return float4(saturate(AmbientLightColor3 + totalLight), 1.0f) * albedo;
/*
    float3 totalLight = float3(0.0f, 0.0f, 0.0f);

    
    float distance = length(lightDirection);
    
    float diffuseLightPercentage = saturate(dot(input.Normal, normalize(lightDirection.xyz)));
    float3 diffuseLight = saturate(DiffuseLightColor3 * diffuseLightPercentage) * intensity;
    totalLight += diffuseLight * intensity / distance;;
    }
        
    
    //float3 lightDirection = -DiffuseLightDirection;
    
    PS_OUTPUT output;
    output.Color = tex.Sample(splr, input.Texture) * input.Color;
    output.Color1 = float4(saturate(AmbientLightColor3 + totalLight), 1.0f);
*/

    return normal;
}
