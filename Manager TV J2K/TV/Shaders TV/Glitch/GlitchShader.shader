Shader "Custom/GlitchShader" {
    Properties {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
        _ColorSplitIntensity ("Color Split", Range(0, 0.1)) = 0.02
        _BlockSize ("Block Size", Range(1, 50)) = 10
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        float _GlitchIntensity;
        float _ColorSplitIntensity;
        float _BlockSize;

        struct Input {
            float2 uv_MainTex;
        };

        float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            // Базовые искажения
            float2 uv = IN.uv_MainTex;
            
            // Блочные искажения
            float2 blockUV = floor(uv * _BlockSize) / _BlockSize;
            float blockRandom = random(blockUV);
            
            // Глитч-смещение
            float glitchOffset = blockRandom * _GlitchIntensity;
            uv.x += sin(uv.y * 10 + _Time.y) * glitchOffset;
            
            // Цветовое разделение
            float3 colorSplit = float3(
                tex2D(_MainTex, uv + float2(_ColorSplitIntensity, 0)).r,
                tex2D(_MainTex, uv).g,
                tex2D(_MainTex, uv - float2(_ColorSplitIntensity, 0)).b
            );
            
            // Добавление шума
            float noise = random(uv * _Time.xy) * _GlitchIntensity;
            
            // Финальный цвет
            o.Albedo = colorSplit + noise;
            o.Alpha = 1.0;
        }
        ENDCG
    }
}