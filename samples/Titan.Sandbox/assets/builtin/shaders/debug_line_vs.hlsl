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
    float4 Position: SV_Position;
};


struct VS_INPUT {
    float3 Position: POSITION;
};    

VS_OUTPUT main(VS_INPUT input) {    
    VS_OUTPUT output;
    output.Position = mul(float4(input.Position, 1.0f), ViewProjection);
    return output;
}
