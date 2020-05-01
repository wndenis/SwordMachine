// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Solid Projector2" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      [PerRendererData]
      _ShadowTex ("Cookie", 2D) = "white" {}
      [PerRendererData]
      _Distance ("Distance", float) = 3
   }
   SubShader {
   Tags {"Queue"="Transparent"}
      Pass {     
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
         
 
         // User-specified properties
         uniform sampler2D _ShadowTex; 
 
         // Projector-specific uniforms
         uniform float4x4 unity_Projector;
         float4x4 unity_ProjectorClip;
         half3 projNormal;
 
          struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float3 normal: TEXCOORD1;
            float4 uvFalloff: TEXCOORD2;
            UNITY_FOG_COORDS(2)
            float4 pos : SV_POSITION;
            float4 posProj : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

            output.pos =  UnityObjectToClipPos(input.vertex);
            output.posProj = mul(unity_Projector, input.vertex);
            output.normal = mul(unity_Projector, input.normal);
            
            //output.normal = normalize(UnityObjectToWorldNormal(input.normal));
            //output.normal = mul(unity_ProjectorClip, input.normal);
            //output.normal = input.normal;
            output.uvFalloff = mul(unity_ProjectorClip, input.vertex);
            output.uvFalloff.a = clamp(dot(normalize(float3(0, 0, -1)), output.normal),-0.5,1);
            UNITY_TRANSFER_FOG(o,o.pos);
            return output;
         }
         
         fixed4 _Color;
         float _Distance;

         float4 frag(vertexOutput input) : COLOR
         {
            //clip(input.uvFalloff);
            if (input.posProj.w > 0.001 && input.normal.z < 0.51)
            {
            //fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(input.uvFalloff));
            //texS.rgba *= _Color.rgba;
               clip(input.posProj.xy / input.posProj.w - 0.001);
               clip(1 - input.posProj.xy / input.posProj.w - 0.001);
               fixed4 cookie = tex2Dproj(_ShadowTex, input.posProj);
               cookie.rgba *= _Color.rgba;
               //return cookie * cookie.a * clamp((input.uvFalloff.a + 0.5) * 2, 0, 1) * clamp(4 - abs(input.posProj.z), 0.0, 1.0);
               
               //if (abs(input.posProj.z) > _Distance - 3){
               //return float4(0.0, 0.0, 0.0, 0.0);
               //}
               float delta = _Distance - abs(input.posProj.z);
               float factor = delta > 0 ? 1 : clamp(delta, 0.0, 1.0);
               fixed4 res = cookie * clamp((input.uvFalloff.a + 0.4) * 2, 0.0, 1.0) * factor;
               UNITY_APPLY_FOG_COLOR(input.fogCoord, res, fixed4(0,0,0,0));
               return res;
            }
            else // behind projector
            {
               return float4(0.0, 0.0, 0.0, 0.0);
               //return float4(0.5, 0.5, 1, 0.0);
            }
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}