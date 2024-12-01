using UnityEngine;

public class SecurityCameraShader : MonoBehaviour
{
    [Header("Shader Material")]
    public Material securityCameraMaterial;

    [Header("Noise Settings")]
    [Range(0f, 1f)] public float noiseIntensity = 0.1f;
    public float noiseSpeed = 5f;

    [Header("Distortion Settings")]
    [Range(0f, 0.1f)] public float distortionIntensity = 0.02f;
    public float distortionSpeed = 2f;

    [Header("Scanline Settings")]
    [Range(0f, 1f)] public float scanlineIntensity = 0.2f;
    public Color scanlineColor = new Color(0, 1, 0, 0.2f);

    [Header("Border Settings")]
    public Color borderColor = Color.black;
    [Range(0f, 1f)] public float borderOpacity = 1f;

    [Header("Vignette Settings")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.3f;

    private void Update()
    {
        if (securityCameraMaterial == null) return;

        // Динамический шум
        float dynamicNoise = Mathf.PingPong(Time.time * noiseSpeed, noiseIntensity);
        securityCameraMaterial.SetFloat("_NoiseIntensity", dynamicNoise);

        // Динамическое искажение
        float dynamicDistortion = Mathf.Sin(Time.time * distortionSpeed) * distortionIntensity;
        securityCameraMaterial.SetFloat("_Distortion", dynamicDistortion);

        // Обновление статических параметров
        securityCameraMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
        securityCameraMaterial.SetColor("_ScanlineColor", scanlineColor);
        securityCameraMaterial.SetColor("_BorderColor", new Color(
            borderColor.r,
            borderColor.g,
            borderColor.b,
            borderOpacity
        ));
        securityCameraMaterial.SetFloat("_VignetteIntensity", vignetteIntensity);
    }

    // Методы для динамического управления
    public void SetNoiseIntensity(float intensity)
    {
        noiseIntensity = Mathf.Clamp01(intensity);
    }

    public void SetDistortion(float distortion)
    {
        distortionIntensity = Mathf.Clamp(distortion, 0f, 0.1f);
    }

    public void SetScanlineColor(Color color)
    {
        scanlineColor = color;
    }
}