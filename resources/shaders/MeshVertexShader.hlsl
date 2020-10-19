cbuffer Camera : register(b0)
{
    matrix View;
    matrix ViewProjection;
    matrix World;
};

struct VS_OUTPUT {
    float3 Normal: Normal;
    float2 Texture: Texture;
    float4 Position: SV_Position;
};

struct VS_INPUT {
    float3 Position: POSITION;
    float3 Normal: Normal;
    float2 Texture: Texture;
};

VS_OUTPUT main(VS_INPUT input) {
    VS_OUTPUT output;

    float4 position = mul(float4(input.Position, 1.0f), World);
    output.Position = mul(position, ViewProjection);
    // output.Position = float4(input.Position, 1.0);
    // output.Position.x += 0.2;

    output.Normal = input.Normal;
    output.Texture = input.Texture;
    //  output.WorldPosition = mul(float4(input.Position, 1.0f), World); 
    // output.Normal = mul(input.Normal, (float3x3) World);

    return output;
}
