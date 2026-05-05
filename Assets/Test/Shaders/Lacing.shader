Shader "Custom/Lacing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LaceAmount ("Lace Amount", Float) = 0.1
        _LaceScale ("Lace Scale", Float) = 1
        _ChromaticAberration ("Chromatic Aberration", Float) = 0
        _Direction ("Direction", Vector) = (1, 0, 0, 0)
        _Seed ("Seed", Float) = 0
        [Toggle] _Reverse ("Reverse", Int) = 0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // VivifyTemplate Libraries
            // #include "Assets/VivifyTemplate/Utilities/Shader Functions/Colors.cginc"
            // #include "Assets/VivifyTemplate/Utilities/Shader Functions/Math.cginc"
            // #include "Assets/VivifyTemplate/Utilities/Shader Functions/Easings.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _LaceAmount;
            float _LaceScale;
            float _ChromaticAberration;
            float _Seed;
            float3 _Direction;
            int _Reverse;
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_Notes);
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, v2f o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float lerp(float a, float b, float t)
            {
                return a + (b-a)*t;
            }

            float4 getScreenCol(float2 uv)
            {
                float caValue = _ChromaticAberration / 100;
                float colorR = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(uv + _Direction.xy * -caValue)).x;
                float colorG = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(uv)).y;
                float colorB = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(uv + _Direction.xy * caValue)).z;
                return float4(colorR, colorG, colorB, 0);
            }

            fixed4 frag(v2f_img i) : SV_Target {

                // if(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_Notes, i.uv).x != 0)
                //     return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_Notes, i.uv);

                float2 dir = normalize(_Direction);
                float2 perpDir = float2(-dir.y, dir.x); // Perpendicular to direction
                float2 uv = i.uv;
            
                // Quantize row index to avoid blur
                float stripeResolution = 300.0;
                float rowIndex = dot(uv, perpDir);
                float quantizedRow = floor(rowIndex * _LaceScale) / _LaceScale;
            
                float offset = (sin(quantizedRow*56456) - 0.5) * 2.0 * _LaceAmount / 100;
            
                uv += dir * offset;
                
                float4 final_col = getScreenCol(uv); 
                
                return final_col;
            }            
            ENDCG
        }
    }
}