// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/SolidProjector3"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowTex ("Cookie", 2D) = "" {}
        _FalloffTex ("FallOff", 2D) = "" {}
        _AngleLimit ("Angle Limit (rad)", Range(0, 6.283)) = 1.55
    }
   
    Subshader
    {
        Tags {"Queue"="Transparent"}
        Pass
        {
            ZWrite Off
            AlphaTest Greater 0
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1
   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
           
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                half projAngle : TEXCOORD2;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
                float4 posProj: TEXCOORD3;
            };
           
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            half3 projNormal;
 
            inline half angleBetween(half3 vector1, half3 vector2)
            {
                return acos(dot(vector1, vector2) / (length(vector1) * length(vector2)));
            }
 
            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (vertex);
                //o.pos = mul(unity_ProjectorClip, vertex);
                o.posProj = mul(unity_Projector, vertex);
                o.uvShadow = mul (unity_Projector, vertex);
                o.uvFalloff = mul (unity_ProjectorClip, vertex);
                projNormal = mul (unity_ProjectorClip, normal);
                o.projAngle = abs(angleBetween(half3(0,0,-1), projNormal));
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
           
            fixed4 _Color;
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            half _AngleLimit;
           
            fixed4 frag (v2f i) : SV_Target
            {
                if (i.projAngle < _AngleLimit && i.posProj.w > 0.001)
                {
                    fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                    texS.rgba *= _Color.rgba;
   
                    fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                    fixed4 res = texS * texF.a;
   
                    UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
                    return res;
                }
                else
                {
                    return fixed4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
}