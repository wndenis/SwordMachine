﻿// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Solid Projector" {
    Properties {
        _Attenuation ("Falloff", Range(0.0, 1.0)) = 1.0
        _ShadowTex ("Cookie", 2D) = "gray" {}
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            Blend SrcColor OneMinusSrcAlpha
            Offset -1, -1
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
         
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 pos : SV_POSITION;
            };
         
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
         
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (vertex);
                o.uvShadow = mul (unity_Projector, vertex);
                return o;
            }
         
            sampler2D _ShadowTex;
            float _Attenuation;
         
            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uvShadow.w > 0.001){
                // Apply alpha mask
                fixed4 texCookie = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                fixed4 outColor = texCookie * texCookie.a; //_Color * texCookie.a;
                // Attenuation
                float depth = i.uvShadow.z; // [-1 (near), 1 (far)]
                return outColor * clamp(1.0 - abs(depth) + _Attenuation, 0.0, 1.0);
                }
                else{
                    return float4(0.0, 0.0, 0.0, 0.0);
                }
            }
            ENDCG
        }
    }
}