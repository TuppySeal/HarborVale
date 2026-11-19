Shader "Hidden/HarborVale/SoftTonemap" {
 Properties { _MainTex ("Source", 2D) = "white" {} _Exposure ("Exposure", Float) = 1 }
 SubShader { Cull Off ZWrite Off ZTest Always Pass {
  CGPROGRAM
  #pragma vertex vert_img
  #pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;half _Exposure;
  fixed4 frag(v2f_img i):SV_Target {half4 source=tex2D(_MainTex,i.uv);half3 x=max(0,source.rgb*_Exposure);half3 mapped=(x*(2.51*x+.03))/(x*(2.43*x+.59)+.14);return half4(saturate(mapped),source.a);}
  ENDCG
 } }
 Fallback Off
}
