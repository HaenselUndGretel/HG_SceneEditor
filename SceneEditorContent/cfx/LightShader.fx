
float2 screen;

float intensity;
float3 color;
float3 position;

// Pointlight 
float radius;

// MapSampler

Texture NormalMap;
sampler NormalMapSampler = sampler_state
 {
	texture = <NormalMap>;
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


Texture DepthMap;
sampler DepthMapSampler = sampler_state
{
  texture = <DepthMap>;
  magfilter = LINEAR;
  minfilter = LINEAR;
  mipfilter = LINEAR;
  AddressU = mirror;
  AddressV = mirror;
};
Texture AoMap;
sampler AoMapSampler = sampler_state
{
  texture = <AoMap>;
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
	//Output.Color = input.color;
	
	return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
  float3 normalColor = tex2D(NormalMapSampler, input.TexCoord.xy).rgb;
  float3 NnormalColor = normalize(normalColor*2.0 - 1.0);

  float ration = screen.y / screen.x;
  float3 nPos = float3(position.xy / screen.xy, position.z);
   float  r =  radius/720;
 // NnormalColor.y *= invert;

  float3 lightDirection = float3((nPos.xy - input.TexCoord.xy), nPos.z);
  lightDirection.y *= ration;

  float3 nLightDirection = normalize(lightDirection);

  float light = clamp(dot(NnormalColor, nLightDirection), 0.0, 1.0);

  float d = sqrt(dot(lightDirection.xy, lightDirection.xy));
  //float att = 1.0/(attenuation.x+attenuation.y*d+attenuation.z*(d*d));

  float dist = distance(nPos, input.TexCoord.xy);
  //float att = falloff(dist);
  float att = 1.0;

  if (dist > r) att = 0.0;

  //float att = 1-d/radius;

  float3 fColor = (light*color*intensity)*att;

    return float4(fColor, 1);



	//float3 nPos = float3(position.xy / screen.xy,position.z);

 //   float3 normalColor = tex2D(NormalMapSampler,input.TexCoord.xy).rgb;

	////normalColor.g = 1.0 - normalColor.g;
	//
	//float3 NnormalColor = normalize(normalColor*2.0-1.0);

	// float3 deltaPos = float3((position-input.TexCoord.xy)/screen.xy,nPos.z);

	// float3 nLightDir = normalize(deltaPos);

	// float light = clamp(dot(NnormalColor,nLightDir),0.0,1.0);

	// //float d = sqrt(dot(deltaPos, deltaPos));  
	//// float att = 1.0 / ( attenuation.x + (attenuation.y*d) + (attenuation.z*d*d) );
 // 
 // float range = saturate(1 - dot(deltaPos / radius, deltaPos / radius));

	//float3 fColor = ((light*color)*range);

 //   return float4(fColor,1);
}



technique Technique1
{
    pass PointLightPass
    {
      
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
