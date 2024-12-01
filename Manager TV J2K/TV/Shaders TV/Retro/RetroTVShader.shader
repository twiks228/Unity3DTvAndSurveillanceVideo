Shader "Custom/RetroTVShader" {
    Properties {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.1
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.2
        _ScanlineFrequency ("Scanline Frequency", Float) = 200
        _ColorShiftR ("Red Color Shift", Range(-1, 1)) = 0
        _ColorShiftB ("Blue Color Shift", Range(-1, 1)) = 0
        _Distortion ("Screen Distortion", Range(0, 0.1)) = 0.02
    }
    SubShader {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        
        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        float _NoiseIntensity;
        float _ScanlineIntensity;
        float _ScanlineFrequency;
        float _ColorShiftR;
        float _ColorShiftB;
        float _Distortion;

        struct Input {
            float2 uv_MainTex;
        };

        float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            // ��������� ������
            float2 distortedUV = IN.uv_MainTex;
            distortedUV.x += sin(IN.uv_MainTex.y * 10.0) * _Distortion;
            
            // ���������� ����
            float noise = random(IN.uv_MainTex * _Time) * _NoiseIntensity;
            
            // ���������
            float scanline = sin(IN.uv_MainTex.y * _ScanlineFrequency) * _ScanlineIntensity;
            
            // �������� �����
            float3 color = tex2D(_MainTex, distortedUV).rgb;
            color.r += _ColorShiftR;
            color.b += _ColorShiftB;
            
            // ��������� ����
            o.Albedo = color + noise - scanline;
            o.Alpha = 1.0;
        }
        ENDCG
    }
}