float3 ambientColor;
float ambientIntensity;

sampler DiffuseMap  : register(s0);
sampler LightMap   : register(s1);
// MapSampler


struct VertexShaderInput
{
    float4 inPos: POSITION0;
	float2 texCoord: TEXCOORD0; 
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};



VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    

   VertexShaderOutput Output = (VertexShaderOutput)0;
	
	Output.Position = input.inPos;
	Output.TexCoord = input.texCoord;	
	return Output;
}



float4 CombinePs(VertexShaderOutput input) : COLOR
{
	float4 colorMap = tex2D(DiffuseMap, input.TexCoord);
	float4 lightMap = tex2D(LightMap, input.TexCoord);
	float3 ambient = ambientColor* ambientIntensity;

	float3 finalColor = (colorMap*lightMap)+(colorMap*ambient);

  //finalColor += colorMap.rgb* ambientColor* ambientIntensity;
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
