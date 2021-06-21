Shader "Custom/ColorMaskOverlay" {

    Properties {
        _MainTex ("Main (RGB)", 2D) = "white" {}
        _Overlay ("Overlay (RGB)", 2D) = "white" {}
        _Mask ("Mask (A)", 2D) = "white" {}
    }
    SubShader {
       	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
       	Blend One One
        LOD 200
     
        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        sampler2D _Overlay;
        sampler2D _Mask;

        struct Input {
            float2 uv_Overlay;
            float2 uv_Mask;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c1 = tex2D (_MainTex, IN.uv_Mask);
            fixed4 c2 = tex2D (_Overlay, IN.uv_Overlay);

            o.Emission = c1.rgb + c2.rgb * c1.rgb/.8;
            o.Alpha = tex2D (_Mask, IN.uv_Mask).a;
        }
        ENDCG
    }
    FallBack "Mobile/Particles/Alpha Blended"
}
