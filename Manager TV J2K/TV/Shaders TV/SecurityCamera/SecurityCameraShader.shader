Shader "Custom/SecurityCameraShader" {
    Properties {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (0,1,0,0.2)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _Distortion ("Camera Distortion", Range(0, 0.1)) = 0.02
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.1
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.3
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.2
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        float4 _ScanlineColor;
        float4 _BorderColor;
        float _Distortion;
        float _NoiseIntensity;
        float _VignetteIntensity;
        float _ScanlineIntensity;

        struct Input {
            float2 uv_MainTex;
        };

        float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            // Базовое искажение
            float2 uv = IN.uv_MainTex;
            
            // Радиальное искажение
            float2 centered = uv - 0.5;
            float dist = length(centered);
            uv += centered * dist * _Distortion;

            // Виньетирование
            float vignette = 1.0 - dist * _VignetteIntensity;
            
            // Развертки
            float scanline = sin(uv.y * 200) * _ScanlineIntensity;
            
            // Шум
            float noise = random(uv * _Time.xy) * _NoiseIntensity;
            
            // Цвет текстуры
            float3 texColor = tex2D(_MainTex, uv).rgb;
            
            // Наложение эффектов
            float3 finalColor = texColor * vignette + 
                                _ScanlineColor.rgb * scanline + 
                                noise;

            // Добавление рамки
            float border = smoothstep(0.45, 0.5, abs(uv.x - 0.5)) + 
                           smoothstep(0.45, 0.5, abs(uv.y - 0.5));
            finalColor = lerp(finalColor, _BorderColor.rgb, border * _BorderColor.a);

            o.Albedo = finalColor;
            o.Alpha = 1.0;
        }
        ENDCG
    }
}