uniform extern texture ScreenTexture;    
sampler TextureSampler : register(s0);
sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};

static const float PI = 3.14159265f;
float wave = PI/0.2f;//0.75f;                // pi/.75 is a good default
float distortion = 1;        // 1 is a good default
float2 centerCoord = {0.5, 0.5};        // 0.5,0.5 is the screen center

float4 PS(float2 texCoord: TEXCOORD0) : COLOR
{
    float2 distance = abs(texCoord - centerCoord);
    float scalar = length(distance);

	//if (scalar > 0.4f)
	//	return tex2D(TextureSampler, texCoord);

    // invert the scale so 1 is centerpoint
    scalar = abs(1.0 - scalar);
        
    // calculate how far to distort for this pixel    
    float sinoffset = sin(wave / scalar);
    sinoffset = clamp(sinoffset, 0, 1);
    
    // calculate which direction to distort
    float sinsign = cos(wave / scalar);    
    
    // reduce the distortion effect
    sinoffset = sinoffset * distortion/64 * pow(((sqrt(2) - length(distance))/sqrt(2)),5);
    
    // pick a pixel on the screen for this pixel, based on
    // the calculated offset and direction
    float4 color = tex2D(TextureSampler, texCoord+(sinoffset*sinsign));    
            
    return color;
}
technique Ripple
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}