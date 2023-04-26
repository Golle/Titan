//#include "common.hlsl"
Texture2D Textures[] : register(t0, space0);
ByteAddressBuffer BufferTable[] : register(t0, space1);

SamplerState LinearSampler : register(s0);
SamplerState PointSampler : register(s1);

struct VSOutput
{
    float4 Position : SV_Position;
    float2 Texture : TEXCOORD;
};

struct SRVIndexConstants 
{
    uint Idx0;
    uint Idx1;
    uint Idx2;
    uint Idx3;
};

ConstantBuffer<SRVIndexConstants> SRVIndices : register(b0);

VSOutput VSMain(in uint VertexIdx : SV_VertexID, in uint InstanceIdx : SV_InstanceID)
{
    VSOutput output;
    if(VertexIdx == 0)
    {
        output.Position = float4(-1.0f, 1.0f, 1.0f, 1.0f);
        output.Texture = float2(0.0f, 0.0f);
    }
    else if(VertexIdx == 1)
    {
        output.Position = float4(3.0f, 1.0f, 1.0f, 1.0f);
        output.Texture = float2(2.0f, 0.0f);
    }
    else
    {
        output.Position = float4(-1.0f, -3.0f, 1.0f, 1.0f);
        output.Texture = float2(0.0f, 2.0f);
    }

    return output;
}


float4 PSMain(in VSOutput input) : SV_Target
{
    Texture2D Texture = Textures[SRVIndices.Idx0];
    return Texture.Sample(LinearSampler, input.Texture);
}