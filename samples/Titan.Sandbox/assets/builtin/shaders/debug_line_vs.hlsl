// cbuffer PerFrameBuffer : register(b0)
// {
//     matrix View;
//     matrix ViewProjection;
// };

cbuffer OrthographicCamera : register(b1)
{
    matrix ViewProjection;
};

struct VS_OUTPUT {
    float4 Color: Color;
    float4 Position: SV_Position;
};


struct VS_INPUT {
    float4 Color: Color;
    float3 Position: POSITION;
    float Nothing: Nothing;
};    

VS_OUTPUT main(VS_INPUT input) {    
    VS_OUTPUT output;
    output.Position = mul(float4(input.Position, 1.0f), ViewProjection);
    output.Color = input.Color;
    return output;
}
