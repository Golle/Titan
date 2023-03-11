Texture2D Textures[] : register(t0, space0);
ByteAddressBuffer BufferTable[] : register(t0, space1);

SamplerState LinearSampler : register(s0);
SamplerState PointSampler : register(s1);


cbuffer PerDrawData : register(b1, space1)
{
    float2 ViewportSize; // do we need this?
    float2 ViewportOffset;
    float2 ViewportScale;
    uint BufferIndex; // the offset in the SRV
    uint LinearSampling; 
}

struct SpriteInstanceData
{
    float2 Offset;
    float2 TextureSize;
    float2 Pivot;
    float2 Scale;
    float4 Color;
    float4 DrawRect;        //Type RectangleF in C#. struct {X,Y, Width, Height} 
    float2 SinCosRotation;
    uint TextureId;
};
StructuredBuffer<SpriteInstanceData> SpriteBuffers[] : register(t0, space2);

struct VSOutput
{
    float4 Position : SV_Position;
    float2 Texture : TEXCOORD;
    float4 Color : COLOR;
    uint TextureId : TextureId;
};


VSOutput VSMain(in uint VertexIdx : SV_VertexID, in uint InstanceIdx : SV_InstanceID)
{
    float2 position = 0.0f;
    float2 texCoords = float2(0.0f, 1.0f);
    if(VertexIdx == 1)
    {
        position = float2(0.0f, 1.0f);
        texCoords = float2(0.0f, 0.0f);
    }
    else if(VertexIdx == 2)
    {
        position = float2(1.0f, 1.0f);
        texCoords = float2(1.0f, 0.0f);
    }
    else if(VertexIdx == 3)
    {
        position = float2(1.0f, 0.0f);
        texCoords = float2(1.0f, 1.0f);
    }

    StructuredBuffer<SpriteInstanceData> spriteBuffer = SpriteBuffers[BufferIndex];
    SpriteInstanceData instanceData = spriteBuffer[InstanceIdx];
    position = position * instanceData.DrawRect.zw * instanceData.Scale;

    float sinRotation = instanceData.SinCosRotation.x;
    float cosRotation = instanceData.SinCosRotation.y;
    float2 pivot = instanceData.Pivot * instanceData.Scale * instanceData.DrawRect.zw;
    
    position -= pivot;
    position = mul(position, float2x2(cosRotation, -sinRotation, sinRotation, cosRotation));
    position += instanceData.Offset;
    
    position -= ViewportOffset;
    
    float2 vp = ViewportSize * ViewportScale;
    position += vp / 2.0f;
    
    position /= vp;
    position = position * 2 - 1;
    
    texCoords *= instanceData.DrawRect.zw/instanceData.TextureSize;
    texCoords += instanceData.DrawRect.xy/instanceData.TextureSize;

    VSOutput output;
    output.Position = float4(position, 0.5, 1.0);
    output.Color = instanceData.Color;
    output.Texture = texCoords;
    output.TextureId = instanceData.TextureId;
    return output;
}

float4 PSMain(in VSOutput input) : SV_Target
{
    if(LinearSampling != 0)
    {
        return Textures[input.TextureId].Sample(LinearSampler, input.Texture) * input.Color;
    }
    else 
    {
        return Textures[input.TextureId].Sample(PointSampler, input.Texture) * input.Color;
    }
}