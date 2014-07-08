float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.
sampler DiffuseMap  : register(s1);
sampler NormalMap	: register(s2);
sampler AoMap		: register(s3);
sampler DepthMap	: register(s4);


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
	float3 WorldPos : TEXCOORD1;
};


struct MRT
{
	float4 ColorTarget : COLOR0;
	float4 NormalTarget: COLOR1;
	float4 AoTarget    : COLOR2;
	float4 DepthTarget : COLOR3;
	float  Depth	   : DEPTH0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
	output.Position.z = input.Position.z;
	output.UV = input.UV;

	output.WorldPos = output.Position.xyz;
	output.WorldPos.z = input.Position.z;

    return output;
}

MRT PixelShaderFunction(VertexShaderOutput input)
{
	MRT output = (MRT)0;

	output.ColorTarget	    = tex2D(DiffuseMap, input.UV);

	output.NormalTarget = tex2D(NormalMap, input.UV);

	//output.NormalTarget.a   = output.ColorTarget.a;

	output.AoTarget         = tex2D(AoMap, input.UV);

	output.DepthTarget      = tex2D(DepthMap, input.UV);
	output.Depth = ((output.DepthTarget.r + output.DepthTarget.g + output.DepthTarget.b) / 3); //* ((1-input.UV.y)*0.80)*input.WorldPos.z;
	output.DepthTarget      = float4(output.Depth, output.Depth, output.Depth, output.DepthTarget.a);
	
	return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
