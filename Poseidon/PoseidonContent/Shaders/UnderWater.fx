sampler ColorMapSampler : register(s0);

//A timer we can use for whatever purpose we want
float fTimer;

float4 PixelShaderGo(float2 Tex: TEXCOORD0) : COLOR
{
	// Use the timer to move the texture coordinated before using them to lookup
	// in the ColorMapSampler. This makes the scene look like its underwater
	// or something similar :)
	
	Tex.x += sin(fTimer+Tex.x*10)*0.01f;
	Tex.y += cos(fTimer+Tex.y*10)*0.01f;
	
	float4 Color = tex2D(ColorMapSampler, Tex);	
	
    return Color;

}

technique PostProcess
{
	pass P0
	{
		// A post process shader only needs a pixel shader.
		PixelShader = compile ps_2_0 PixelShaderGo();
	}
}