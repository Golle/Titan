
struct VS_OUTPUT {
    float4 Position: SV_POSITION;
    float2 Texture: Texture;
    float4 Color: Color;
};

struct VS_INPUT {
    float3 Position : Position0;
    float2 Texture : Texture0;
    float4 Color : Color0;
    float3 InstancePosition : InstancePosition1;
};

VS_OUTPUT main(VS_INPUT input) {
    VS_OUTPUT output;
    output.Position = float4(input.Position + input.InstancePosition, 1.0);
    // output.Position = float4(input.Position, 1.0);
    // output.Color = input.Color;
    output.Color = input.Color;
    output.Texture = input.Texture;
    return output;
}
