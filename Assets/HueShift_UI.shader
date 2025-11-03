Shader "UI/HueShift"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Hue ("Hue Shift (Degrees)", Range(-180,180)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Hue;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed3 HueShift(fixed3 color, float hueDegrees)
            {
                float hue = radians(hueDegrees);
                float cosH = cos(hue);
                float sinH = sin(hue);

                float3x3 mat = float3x3(
                    0.299 + 0.701 * cosH + 0.168 * sinH, 0.587 - 0.587 * cosH + 0.330 * sinH, 0.114 - 0.114 * cosH - 0.497 * sinH,
                    0.299 - 0.299 * cosH - 0.328 * sinH, 0.587 + 0.413 * cosH + 0.035 * sinH, 0.114 - 0.114 * cosH + 0.292 * sinH,
                    0.299 - 0.300 * cosH + 1.250 * sinH, 0.587 - 0.588 * cosH - 1.050 * sinH, 0.114 + 0.886 * cosH - 0.203 * sinH
                );

                return saturate(mul(mat, color));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb = HueShift(c.rgb, _Hue);
                return c;
            }
            ENDCG
        }
    }
}
