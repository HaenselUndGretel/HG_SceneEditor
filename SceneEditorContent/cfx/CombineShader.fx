float3 ambientColor;
float ambientIntensity;


// MapSampler

Texture LightMap;
sampler LightMapSampler = sampler_state
 {
	texture = <LightMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture ColorMap;
sampler ColorMapSampler = sampler_state 
{
	texture = <ColorMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};



struct VertexShaderInput
{
    float4 inPos: POSITION0;
	float2 texCoord: TEXCOORD0; 
	//float4 color: COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	//float4 Color : COLOR0;
};



VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    

   VertexShaderOutput Output = (VertexShaderOutput)0;
	
	Output.Position = input.inPos;
	Output.TexCoord = input.texCoord;
	///Output.Color = input.color;
	
	return Output;
}



float4 CombinePs(VertexShaderOutput input) : COLOR
{
  float4 colorMap = tex2D(ColorMapSampler, input.TexCoord);
  float4 lightMap = tex2D(LightMapSampler, input.TexCoord);

  float3 finalColor = colorMap.rgb* lightMap.rgb;

  finalColor += colorMap.rgb* ambientColor* ambientIntensity;
    return float4(finalColor,1);
}



technique Technique1
{
	pass CombinePass
	{
		 VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 CombinePs();
	}
}
