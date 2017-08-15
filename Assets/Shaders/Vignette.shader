Shader "Hidden/Vignette"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Power("Vignette power", Range(0.0, 10.0)) = 5.0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			float _Power;

			fixed4 frag (v2f_img i) : COLOR
			{
				float4 renderTex = tex2D(_MainTex, i.uv);
				float2 dist = (i.uv - 0.5f) * 1.25f;
				dist.x = pow(dot(dist, dist), 10.3f - _Power);
				dist.x = clamp(dist.x, 0.0f, 1.0f);
				renderTex.rgb = (1.0f - dist.x) * renderTex.rgb + (dist.x) * _Color;
				return renderTex;
			}
			ENDCG
		}
	}
}
