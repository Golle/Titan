namespace Titan.Tools.Core.Models.WavefrontObj.Lexer;

internal enum TokenType
{
    Unknown,
    Invalid,

    // Built in types
    VertexPosition,
    VertexNormal,
    TextureCoordinates,
    ParameterSpace,
    Line,
    Face,   
    UseMaterial,
    Object,
    Group,
    Smooth,
    NewMaterial,
    MaterialLib,    
    AmbientColor,
    DiffuseColor,
    SpecularColor,
    EmissiveColor,
    SpecularExponent,
    Illumination,
    Transparent,    
    TransparentInverted,
    TransmissionFilter,
    OpticalDensity,

    // texture types
    DiffuseTexture,
    AmbientTexture,
    SpecularTexture,
    DisplacementTexture,



    // Types
    Number,
    Identifier,

    // Operators
    Minus,
    Slash,
    


    

    // Special
    NewLine,
    Whitespace,
    Comment,
    




    // EOF
    EndOfFile,// End of the file


}
