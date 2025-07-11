Shader "Custom/WarpedBackground"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Distortion Intensity", Range(0, 0.2)) = 0.1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseOffset ("Noise Offset", Vector) = (0, 0, 0, 0)
        _TimeValue ("Time", Float) = 0
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
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            sampler2D _MainTex;
            float _Intensity;
            float _NoiseScale;
            float2 _NoiseOffset;
            float _TimeValue;
            
            // 간단한 노이즈 함수
            float noise(half2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            // 부드러운 노이즈
            float smoothNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                // 4개의 코너에서 노이즈 샘플링
                float a = noise(i);
                float b = noise(i + float2(1.0, 0.0));
                float c = noise(i + float2(0.0, 1.0));
                float d = noise(i + float2(1.0, 1.0));
                
                // 보간
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + 
                       (c - a) * u.y * (1.0 - u.x) + 
                       (d - b) * u.x * u.y;
            }
            
            // 프랙탈 브라운 모션 (더 자연스러운 노이즈)
            float fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                
                for (int i = 0; i < 3; i++)
                {
                    value += amplitude * smoothNoise(uv);
                    uv *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 시간에 따라 변화하는 노이즈 오프셋
                float2 uv = IN.texcoord * _NoiseScale + _NoiseOffset;
                
                // 노이즈 생성
                float noise1 = fbm(uv + _TimeValue * 0.3);
                float noise2 = fbm(uv + _NoiseOffset + _TimeValue * 0.2);
                
                // UV 왜곡
                float2 distortion = float2(
                    (noise1 - 0.5) * 2.0 * _Intensity,
                    (noise2 - 0.5) * 2.0 * _Intensity
                );
                
                // 왜곡된 UV로 텍스처 샘플링
                half4 color = tex2D(_MainTex, IN.texcoord + distortion);
                
                // 클리핑 및 컬러 적용
                color *= IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                
                return color;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
