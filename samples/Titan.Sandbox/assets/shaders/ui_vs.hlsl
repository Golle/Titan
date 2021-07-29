cbuffer OrthographicCamera : register(b1)
{
    matrix ViewProjection;
};

struct VS_OUTPUT {
    float2 Texture: Texture;
    float4 Position: SV_Position;
};

struct VS_INPUT {
    float2 Position: POSITION;
    float2 Texture: Texture;
};    


VS_OUTPUT main(VS_INPUT input) { 
    VS_OUTPUT output;
    output.Position = mul(float4(input.Position, 1.0f, 1.0f), ViewProjection);
    output.Texture = input.Texture;
    return output;
}
