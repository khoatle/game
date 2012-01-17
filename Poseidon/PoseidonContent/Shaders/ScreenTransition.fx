
sampler ColorMapSampler : register(s0);

texture ColorMap2;
sampler ColorMapSampler2 = sampler_state
{
   Texture = <ColorMap2>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

float fFadeAmount;

// Transition
float4 PS(float2 Tex: TEXCOORD0) : COLOR
{
	float4 Color = tex2D(ColorMapSampler, Tex);	
	float4 Color2 = tex2D(ColorMapSampler2, Tex);	
	
	float4 finalColor = lerp(Color,Color2,fFadeAmount);
	
	// Set our alphachannel to fAlphaAmount.
	finalColor.a = 1;
		
    return finalColor;
}

technique PostProcess
{
	pass P0
	{
		// A post process shader only needs a pixel shader.
		PixelShader = compile ps_2_0 PS();
	}
}