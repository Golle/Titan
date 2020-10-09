struct PS_INPUT
{
    float4 Position: SV_POSITION;
    float4 Color: Color;
};

float4 main(PS_INPUT input) : SV_TARGET
{
 	return input.Color;
}
