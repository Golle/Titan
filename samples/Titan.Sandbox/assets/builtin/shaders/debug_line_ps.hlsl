
struct PS_INPUT {
    float4 Color: Color;
};

float4 main(PS_INPUT input) : SV_TARGET
{
    return input.Color;
    return float4(1,1,0,0);
}
