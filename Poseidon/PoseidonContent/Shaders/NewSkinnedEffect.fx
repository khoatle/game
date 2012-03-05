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
float4 AmbientColor = float4(0, 191.0f / 255.0f, 1, 1);
//light sky blue: 135-206-250
//deep sky blue: float4(0, 191.0f / 255.0f, 1, 1);
//corn flower blue: float4(100.0f / 255.0f, 149.0f / 255.0f, 237.0f / 255.0f, 1);
//blue: float4(0, 0, 1.0, 1.0);

//--------------------------- DIFFUSE LIGHT PROPERTIES ------------------------------ 
// The direction of the diffuse light 
float3 DiffuseLightDirection = float3(0, 1, 0); 
  
// The color of the diffuse light 
float4 DiffuseColor = float4(0, 1, 1, 1);
  
// The intensity of the diffuse light 
float DiffuseIntensity = 1.0f; 

//--------------------------- SPECULAR LIGHT PROPERTIES ------------------------------ 
//Camera position
float4 EyePosition;
//Color
float4 SpecularColor = float4(135.0f / 255.0f, 206.0f / 255.0f, 250.0f / 255.0f, 1);
//Shininess
float Shininess = 1.0f;
  
//--------------------------- FOG PROPERTIES ------------------------------ 
float3 FogColor;   
float4 FogVector;

//--------------------------- BALLOON EFFECT PROPERTIES ------------------------------
float4x4 gWorldXf;
float4x4 gWorldITXf;
float4x4 gWvpXf;
float4x4 gViewIXf;
/* data from application vertex buffer */
float gInflate = 1.0f;//0.06f;
float3 gGlowColor = {1.0f, 0.9f, 0.3f};
float gGlowExpon = 1.5;

 
//--------------------------- HELPER FUNCTIONS ------------------------------ 
float ComputeFogFactor(float4 position)
{
    return saturate(dot(position, FogVector));
}
void ApplyFog(inout float4 color, float fogFactor)
{
    color.rgb = lerp(color.rgb, FogColor * color.a, fogFactor);
}
  
//--------------------------- TEXTURE PROPERTIES ------------------------------ 
// The texture being used for the object 
texture Texture; 
  
// The texture sampler, which will get the texture color 
sampler2D textureSampler = sampler_state  
{ 
    Texture = (Texture); 
    MinFilter = Linear; 
    MagFilter = Linear; 
    AddressU = Wrap;//Clamp; 
    AddressV = Wrap;//Clamp; 
}; 
  
//--------------------------- DATA STRUCTURES ------------------------------ 
// The structure used to store information between the application and the 
// vertex shader 
struct AppToVertex 
{ 
    float4 Position : SV_Position; 
    float3 Normal   : NORMAL; 
    float2 TexCoord : TEXCOORD0; 
    int4   Indices  : BLENDINDICES0; 
    float4 Weights  : BLENDWEIGHT0; 
}; 

// The structure used to store information between the vertex shader and the 
// pixel shader 
struct VertexToPixel 
{ 
	
	float2 TextureCoordinate : TEXCOORD0; 
	float4 PositionWS : TEXCOORD1; 
    float3 NormalWS : TEXCOORD2; 
	float4 PositionPS : SV_Position;
	//float3 LightDir : TEXCOORD2;
    //float3 ViewDir : TEXCOORD3;
	//float FogFactor : TEXCOORD4;
}; 

//--------------------------- DATA STRUCTURES FOR BALLOON EFFECT ------------------------------ 

struct appdata {
    float4 Position	: POSITION;
    float4 UV		: TEXCOORD0;
    float3 Normal	: NORMAL;
    float4 Tangent	: TANGENT0;
    float4 Binormal	: BINORMAL0;
	int4   Indices  : BLENDINDICES0; 
    float4 Weights  : BLENDWEIGHT0; 
};

/* data passed from vertex shader to pixel shader */
struct gloVertOut {
    float4 HPosition	: POSITION;
    float3 WorldNormal	: TEXCOORD0;
    float3 WorldView	: TEXCOORD1;
};

 //SKIN Method 
 
void Skin(inout AppToVertex vin, uniform int boneCount) 
{ 
    float4x3 skinning = 0; 
 
    [unroll] 
    for (int i = 0; i < boneCount; i++) 
    { 
        skinning += Bones[vin.Indices[i]] * vin.Weights[i]; 
    } 
 
    vin.Position.xyz = mul(vin.Position, skinning); 
    vin.Normal = mul(vin.Normal, (float3x3)skinning); 
}
void SkinBalloon(inout appdata vin, uniform int boneCount) 
{ 
    float4x3 skinning = 0; 
 
    [unroll] 
    for (int i = 0; i < boneCount; i++) 
    { 
        skinning += Bones[vin.Indices[i]] * vin.Weights[i]; 
    } 
 
    vin.Position.xyz = mul(vin.Position, skinning); 
    vin.Normal = mul(vin.Normal, (float3x3)skinning); 
}
  
//--------------------------- SHADERS ------------------------------ 

// A completely normal vertex shader
VertexToPixel NothingSpecialVertexShader(AppToVertex input) 
{ 
    VertexToPixel output; 
    Skin(input, 4); 
 
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
	Skin(input, 4);

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

//--------------------------- SHADERS FOR BALLOON EFFECT ------------------------------ 

/*********** vertex shader ******/

gloVertOut gloBalloon_VS(appdata IN,
    uniform float Inflate,
    uniform float4x4 WorldITXf, // our four standard "untweakable" xforms
	uniform float4x4 WorldXf,
	uniform float4x4 ViewIXf,
	uniform float4x4 WvpXf
) {
    gloVertOut OUT = (gloVertOut)0;
	SkinBalloon(IN, 4); 
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;
    float4 Po = float4(IN.Position.xyz,1);
    Po += (Inflate*normalize(float4(IN.Normal.xyz,0))); // the balloon effect
    float4 Pw = mul(Po,WorldXf);
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);
    return OUT;
}

/********* pixel shaders ********/

float4 gloBalloon_PS(gloVertOut IN,
    uniform float3 GlowColor,
    uniform float GlowExpon
) : COLOR {
    float3 Nn = normalize(IN.WorldNormal);
    float3 Vn = normalize(IN.WorldView);
    float edge = 1.0 - dot(Nn,Vn);
    edge = pow(edge,GlowExpon);
    float3 result = edge * GlowColor.rgb;
    return float4(result,edge);
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

// Balloon effect technique
technique BalloonShading
{
    pass GlowPass     	
    {
        VertexShader = compile vs_2_0 gloBalloon_VS(gInflate,gWorldITXf,gWorldXf,gViewIXf,gWvpXf);
		ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		CullMode = CCW;
        PixelShader = compile ps_2_0 gloBalloon_PS(gGlowColor,gGlowExpon);
    }
}