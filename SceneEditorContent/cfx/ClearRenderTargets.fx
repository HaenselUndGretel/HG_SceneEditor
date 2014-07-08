
struct GBuffer
{
	float4 Diffuse	: COLOR0;
	float4 Normal	: COLOR1;
	float4 Ao		: COLOR2;
	float4 Depth	: COLOR3;
};


float4 VS_Main(float3 Position : POSITION0) : POSITION0
{
	return float4(Position, 1);
}

GBuffer PS_Main()
{
	GBuffer output = (GBuffer)1;

	output.Diffuse.rgba	= 0.0f;
	output.Normal.rgb	= float3(0.5,0.5,1.0);
	output.Ao			= 1.0f;
	output.Depth.rgb	= 0.0f;

	return output;
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader = compile ps_2_0 PS_Main();
    }
}
