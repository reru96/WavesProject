Shader "Custom/PaintBrush"
{
    Properties
    {
        _MainTex ("Paint Texture", 2D) = "white" {}   // la RenderTexture
        _PaintColor ("Paint Color", Color) = (1,1,1,1)
        _PaintUV ("Paint UV", Vector) = (0,0,0,0)
        _BrushSize ("Brush Size", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _PaintColor;
            float4 _PaintUV;
            float _BrushSize;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);

                float dist = distance(i.uv, _PaintUV.xy);

                float mask = saturate(1 - dist / _BrushSize);

                return lerp(baseCol, _PaintColor, mask);
            }
            ENDCG
        }
    }
}
