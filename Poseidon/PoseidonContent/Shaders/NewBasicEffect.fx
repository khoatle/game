//--------------------------- BASIC PROPERTIES ------------------------------ 
#define MaxBones 72 
 
float4x3 Bones[MaxBones]; 
// The world transformation 
float4x4 World; 
  
// The view transformation 
float4x4 View; 
  
// The projection transformation 
float4x4 Projection; 
  
// The transpose of the inverse of the world transformation, 
// used for transforming the vertex's normal 
float4x4 WorldInverseTranspose; 

//--------------------------- AMBIENT LIGHT PROPERTIES ------------------------------
//Intensity 
float AmbientIntensity = 0.8f;
//Color
float4 AmbientColor = float4(0.075, 0.075, 0.2, 1.0);

//--------------------------- DIFFUSE LIGHT PROPERTIES ------------------------------ 
// The direction of the diffuse light 
float3 DiffuseLightDirection = float3(0, 1, 0); 
  
// The color of the diffuse light 
float4 DiffuseColor = float4(1, 1, 1, 1); 
  
// The intensity of the diffuse light 
float DiffuseIntensity = 1.0f; 

//--------------------------- SPECULAR LIGHT PROPERTIES ------------------------------ 
//Camera position
float4 EyePosition;
//Color
float4 SpecularColor = float4(1, 1, 1, 1);
//Shininess
float Shininess = 15.0f;
  
//--------------------------- FOG PROPERTIES ------------------------------ 
float3 FogColor;   
float4 FogVector;
 
//--------------------------- HELPER FUNCTIONS ------------------------------ 
float ComputeFogFactor(float4 position)
{
    return saturate(dot(position, FogVector));
}
void ApplyFog(inout float4 color, float fogFactor)
{
    color.rgb = lerp(color.rgb, FogColor * color.a, fogFactor);
}

//--------------------------- TOON SHADER PROPERTIES ------------------------------ 
// The color to draw the lines in.  Black is a good default. 
float4 LineColor = float4(0, 0, 0, 1); 
  
// The thickness of the lines.  This may need to change, depending on the scale of 
// the objects you are drawing. 
float4 LineThickness = 0.12; 
  
//--------------------------- TEXTURE PROPERTIES ------------------------------ 
// The texture being used for the object 
texture Texture; 
  
// The texture sampler, which will get the texture color 
sampler2D textureSampler = sampler_state  
{ 
    Texture = (Texture); 
    MinFilter = Linear; 
    MagFilter = Linear; 
    AddressU = Clamp; 
    AddressV = Clamp; 
}; 
  
//--------------------------- DATA STRUCTURES ------------------------------ 
// The structure used to store information between the application and the 
// vertex shader 
struct AppToVertex 
{ 
    float4 Position : SV_Position; 
    float3 Normal   : NORMAL; 
    float2 TexCoord : TEXCOORD0; 
}; 

// The structure used to store information between the vertex shader and the 
// pixel shader 
struct VertexToPixel 
{ 
	
	float2 TextureCoordinate : TEXCOORD0; 
	float4 PositionWS : TEXCOORD1; 
    float3 NormalWS : TEXCOORD2; 
	float4 PositionPS : SV_Position;
}; 
  
//--------------------------- SHADERS ------------------------------ 

// A completely normal vertex shader
VertexToPixel NothingSpecialVertexShader(AppToVertex input) 
{ 
    VertexToPixel output; 

    // Transform the position 
    float4 worldPosition = mul(input.Position, World); 
    float4 viewPosition = mul(worldPosition, View); 
    output.PositionPS = mul(viewPosition, Projection); 
    float3 Pos_ws = mul(input.Position, World).xyz;

    // Transform the normal 
    output.NormalWS = normalize(mul(input.Normal, WorldInverseTranspose)); 
 
    // Copy over the texture coordinate 
    output.TextureCoordinate = input.TexCoord; 

	// compute fog factor
	float FogFactor = ComputeFogFactor(input.Position); 
    output.PositionWS = float4(Pos_ws, FogFactor);

    return output; 
} 
  
// A normal pixel shader 
float4 NothingSpecialPixelShader(VertexToPixel input) : COLOR0 
{ 
    float3 Normal = normalize(input.NormalWS);
    float3 LightDir = normalize(DiffuseLightDirection);
    float3 ViewDir = normalize(EyePosition - input.PositionWS.xyz);   
    
    float Diff = saturate(dot(Normal, LightDir));
	float3 Reflect = normalize(2 * Diff * Normal - LightDir); 
    float Specular = pow(saturate(dot(Reflect, ViewDir)), Shininess);
	   
    float4 color = tex2D(textureSampler, input.TextureCoordinate);

    color *= (AmbientIntensity * AmbientColor + DiffuseIntensity * DiffuseColor * Diff + SpecularColor * Specular);
	color.a = 1;

	ApplyFog(color, input.PositionWS.w);

    return color; 
}

// Output structure for the vertex shader that renders normal and depth information.
struct NormalDepthVertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

NormalDepthVertexShaderOutput NormalDepthVertexShader(AppToVertex input)
{
    NormalDepthVertexShaderOutput output;

    // Apply camera matrices to the input position.
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    
    float3 worldNormal = mul(input.Normal, World);

    // The output color holds the normal, scaled to fit into a 0 to 1 range.
    output.Color.rgb = (worldNormal + 1) / 2;

    // The output alpha holds the depth, scaled to fit into a 0 to 1 range.
    output.Color.a = output.Position.z / output.Position.w;
    
    return output;    
}
// Simple pixel shader for rendering the normal and depth information.
float4 NormalDepthPixelShader(float4 color : COLOR0) : COLOR0
{
    return color;
}

// Normal here means not abnormal
technique NormalShading
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 NothingSpecialVertexShader(); 
        PixelShader = compile ps_2_0 NothingSpecialPixelShader(); 
		//CullMode = CCW;
	}
}

// Technique draws the object as normal and depth values.
technique NormalDepth
{
    pass P0
    {
        VertexShader = compile vs_2_0 NormalDepthVertexShader();
        PixelShader = compile ps_2_0 NormalDepthPixelShader();
    }
}
