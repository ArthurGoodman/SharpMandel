float2 resolution;

int maxIt;
int samples;
float2 offset;
float scale;

float2 seed;

float4 mandel(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
	float3 col = 0;

	for (int sx = 0; sx < samples; sx++) {
		for (int sy = 0; sy < samples; sy++) {
			float2 c = (texCoord - 0.5 + float2((float)sx, (float)sy) / (float)samples / resolution) / scale * float2(1, resolution.y / resolution.x) - offset;

				float q = (c.x - .25)*(c.x - .25) + c.y*c.y;

			if (q*(q + (c.x - .25)) < .25*c.y*c.y)
				continue;

			float n = 0;
			float2 z = 0;

				for (int i = 0; i < maxIt; i++) {
					z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + c;

					if (dot(z, z) > 8)
						break;

					n++;
				}

			if (n < maxIt)
				col += .5 + .5 * cos(float3(3, 4, 11) + .05 * (n - log2(log2(dot(z, z)))));
		}
	}

	col /= (float)(samples * samples);

	return float4(col, 1);
}

float4 julia(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
	float3 col = 0;

	for (int sx = 0; sx < samples; sx++) {
		for (int sy = 0; sy < samples; sy++) {
			float2 c = (texCoord - 0.5 + float2((float)sx, (float)sy) / (float)samples / resolution) / scale * float2(1, resolution.y / resolution.x) - offset;

				float q = (c.x - .25)*(c.x - .25) + c.y*c.y;

			float n = 0;
			float2 z = c;

				for (int i = 0; i < maxIt; i++) {
					z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + seed;

					if (dot(z, z) > 8)
						break;

					n++;
				}

			if (n < maxIt)
				col += .5 + .5 * cos(float3(3, 4, 11) + .05 * (n - log2(log2(dot(z, z)))));
		}
	}

	col /= (float)(samples * samples);

	return float4(col, 1);
}

float4 combined(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
	return mandel(position, color, texCoord) * 0.2 + julia(position, color, texCoord) * 0.8;
}

technique FractalTechnique {
	pass MandelPass {
		PixelShader = compile ps_5_0 mandel();
	}

	pass JuliaPass {
		PixelShader = compile ps_5_0 julia();
	}

	pass CombinedPass {
		PixelShader = compile ps_5_0 combined();
	}
}
