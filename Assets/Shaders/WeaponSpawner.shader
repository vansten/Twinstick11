Shader "Unlit/WeaponSpawner"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		Cull Front
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 vertexModel : TEXCOORD0;
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertexModel = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				col.a = col.a * clamp(pow(1.0f - i.vertexModel.y, 0.75f), 0, 1);
				return col;
			}
			ENDCG
		}
	}
}
