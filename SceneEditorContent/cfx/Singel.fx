float4x4 World;
float4x4 View;
float4x4 Projection;

sampler Diffuse	: register(s1);

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
	output.UV = input.UV;
    

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
  float4 fColor = tex2D(Diffuse, input.UV);

	return fColor;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
