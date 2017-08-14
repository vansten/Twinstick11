Shader "Unlit/Crosshair"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv :TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv :TEXCOORD0;
			};

			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				float2 uv = i.uv - float2(0.5f, 0.5f);
				float dist = sqrt(dot(uv, uv));
				col.a = clamp(pow(sin(dist * 5.0f), 8.0f), 0, 1);
				return col;
			}
			ENDCG
		}
	}
}
