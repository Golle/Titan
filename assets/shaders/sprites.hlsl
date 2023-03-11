Texture2D Textures[] : register(t0, space0);
ByteAddressBuffer BufferTable[] : register(t0, space1);

SamplerState LinearSampler : register(s0);
SamplerState PointSampler : register(s1);

cbuffer PerDrawData : register(b1, space1)
{
    float2 ViewportSize;
    float2 Padding; // WHY?!
    uint BufferIndex;
    uint LinearSampling;
}

struct SpriteDrawData
{
    float2 PositionOffset;
    float2 Scale;
    float2 Pivot;
    float2 TextureSize;     // SizeF in C# struct {Width, Height}
    float4 InstanceColor;
    float4 DrawRect;        //Type RectangleF in C#. struct {X,Y, Width, Height} 
    float2 SinCosRotation;
    uint TextureId;
};

StructuredBuffer<SpriteDrawData> SpriteBuffers[] : register(t0, space2);
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
    
    StructuredBuffer<SpriteDrawData> spriteBuffer = SpriteBuffers[BufferIndex];
    SpriteDrawData instanceData = spriteBuffer[InstanceIdx];
    
    float sinRotation = instanceData.SinCosRotation.x;
    float cosRotation = instanceData.SinCosRotation.y;
    float2 pivot = instanceData.Pivot * instanceData.Scale * instanceData.DrawRect.zw;

    float2 positionSS = position * instanceData.DrawRect.zw;
    positionSS *= instanceData.Scale;
    positionSS -= pivot;
    positionSS = mul(positionSS, float2x2(cosRotation, -sinRotation, sinRotation, cosRotation));
    positionSS += instanceData.PositionOffset;
    

    float2 positionDS = positionSS;
    positionDS /= ViewportSize;
    positionDS = positionDS * 2 - 1;
    // positionDS.y *= -1;

    texCoords.xy *= instanceData.DrawRect.zw / instanceData.TextureSize;
    texCoords.xy += instanceData.DrawRect.xy / instanceData.TextureSize;

    VSOutput output;
    output.Texture = texCoords;
    output.TextureId = instanceData.TextureId;
    output.Position = float4(positionDS, 0.5f, 1.0);
    output.Color = instanceData.InstanceColor; //float4(instanceData.InstanceColor.x,1.0f, 1.0f, 1.0f);
    return output;
}

float4 PSMain(in VSOutput input) : SV_Target
{
    // return float4(1.0, 1.0, 1.0, 1.0);
    float4 theColor = input.Color;
    if(LinearSampling != 0){
        return Textures[input.TextureId].Sample(LinearSampler, input.Texture) * input.Color;
    }
    else {
        return Textures[input.TextureId].Sample(PointSampler, input.Texture) * input.Color;
    }
}