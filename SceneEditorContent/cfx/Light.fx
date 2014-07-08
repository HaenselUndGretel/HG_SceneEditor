float4x4 World;
float4x4 View;
float4x4 Projection;

sampler NormalMap  : register(s0);
sampler DepthMap   : register(s1);


float2 screen;

float  LightIntensity;
float3 LightColor;
float4 LightPosition;
float  LightRadius;

struct VertexShaderInput
{
    float4 Position   : POSITION0;
	  float2 TexCoord		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position   : POSITION0;
	  float2 TexCoord		: TEXCOORD0;
    float4 LightPos   : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
  VertexShaderOutput Output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 ProjectionPosition = mul(viewPosition, Projection);

	Output.Position = input.Position;
  Output.LightPos = mul(LightPosition, View);
	Output.TexCoord = input.TexCoord;

	return Output;
}

float falloff(float distanc)
{
	float maxDistance = 10.00;
	float scalingFactor = 1.0;

	if (distanc <= maxDistance / 3.0)
	{
		return scalingFactor * (1.0 - 3.0 * distanc * distanc / (maxDistance * maxDistance));
	}
	else if (distanc <= maxDistance)
	{
		float x = 1.0 - distanc / maxDistance;
		return (3.0 / 2.0) * scalingFactor * x * x;
	}
		return 0.0;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

		//Maps Laden
		float4 normalColor = tex2D(NormalMap, input.TexCoord.xy).rgba;
		float AoColor = normalColor.a;
		float3 DepthColor = tex2D(DepthMap, input.TexCoord.xy).rgb;

		// Normalen normalisiren (RGB) = (XYZ)
		float3 normal = normalColor.rgb;
		float3 NnormalColor = normalize(normal*2.0 - 1.0);
		//NnormalColor.y *= -1;

		float ration = screen.y / screen.x;

		float z = ((DepthColor.r + DepthColor.g + DepthColor.b) / 3)*650 ;


		float3 WorldPos = float3(screen.x*input.TexCoord.x, (screen.y*input.TexCoord.y) + (z*0.80), z);
			// WorldPos = float3(input.TexCoord.x, (input.TexCoord.y) + (z*0.80), z);


			float3 LightPos = input.LightPos.xyz;
		//float3 LightPos = float3(LightPosition.x, LightPosition.y , LightPosition.z * 720);


		float3 lightDir = (LightPos - WorldPos) * float3(0.75, 1.0, 1.0);

		

		//
		float light = max(dot(NnormalColor, normalize(lightDir)), 0.0) ;

		float lightDistance = length(lightDir);
		//float attenuation = LightIntensity / (max(lightDistance * LightRadius, 1.0 / LightRadius));
    float attenuation = LightIntensity / (lightDistance + 0.001) - (1 / LightRadius);
		//float attenuation =saturate(1/  lightDistance * LightRadius);

		//float attenuation =LightRadius/ (LightRadius+lightDistance);

		float3 fColor = LightColor*LightIntensity*light*attenuation;

		return float4(fColor, 1);
		/*float3 LightDir = (WorldPos - LightPos);
		float3 NLightDir = normalize(LightDir);


		float light = saturate(dot(NnormalColor, -NLightDir));

		float distance = length(LightDir);
		
		float att = saturate(LightRadius / distance);*/


		
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
