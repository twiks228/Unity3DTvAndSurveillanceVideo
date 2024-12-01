using UnityEngine;

public class RetroTVEffect : MonoBehaviour
{
    [Header("Material Settings")]
    public Material retroTVMaterial;

    [Header("Noise Settings")]
    [Range(0f, 1f)] public float noiseIntensity = 0.1f;
    [Range(0f, 1f)] public float noiseSpeed = 0.5f;

    [Header("Scanline Settings")]
    [Range(0f, 1f)] public float scanlineIntensity = 0.2f;
    [Range(0f, 10f)] public float scanlineFrequency = 200f;

    [Header("Color Shift Settings")]
    [Range(0f, 1f)] public float colorShiftIntensity = 0.05f;
    [Range(0f, 1f)] public float colorShiftSpeed = 0.5f;

    [Header("Distortion Settings")]
    [Range(0f, 0.1f)] public float distortionIntensity = 0.02f;
    [Range(0f, 10f)] public float distortionFrequency = 10f;

    [Header("Dynamic Effects")]
    public bool enableDynamicNoise = true;
    public bool enableDynamicScanlines = true;
    public bool enableColorShift = true;
    public bool enableDistortion = true;

    private void Update()
    {
        if (retroTVMaterial == null) return;

        // Динамический шум
        if (enableDynamicNoise)
        {
            float dynamicNoise = Mathf.PingPong(Time.time * noiseSpeed, noiseIntensity);
            retroTVMaterial.SetFloat("_NoiseIntensity", dynamicNoise);
        }
        else
        {
            retroTVMaterial.SetFloat("_NoiseIntensity", noiseIntensity);
        }

        // Динамические развертки
        if (enableDynamicScanlines)
        {
            float dynamicScanline = 0.1f + Mathf.Sin(Time.time) * scanlineIntensity;
            retroTVMaterial.SetFloat("_ScanlineIntensity", dynamicScanline);
            retroTVMaterial.SetFloat("_ScanlineFrequency", scanlineFrequency);
        }
        else
        {
            retroTVMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
        }

        // Цветовой сдвиг
        if (enableColorShift)
        {
            float redShift = Mathf.Sin(Time.time * colorShiftSpeed) * colorShiftIntensity;
            float blueShift = Mathf.Cos(Time.time * colorShiftSpeed) * colorShiftIntensity;

            retroTVMaterial.SetFloat("_ColorShiftR", redShift);
            retroTVMaterial.SetFloat("_ColorShiftB", blueShift);
        }
        else
        {
            retroTVMaterial.SetFloat("_ColorShiftR", 0f);
            retroTVMaterial.SetFloat("_ColorShiftB", 0f);
        }

        // Искажение экрана
        if (enableDistortion)
        {
            float dynamicDistortion = Mathf.Sin(Time.time * distortionFrequency) * distortionIntensity;
            retroTVMaterial.SetFloat("_Distortion", dynamicDistortion);
        }
        else
        {
            retroTVMaterial.SetFloat("_Distortion", distortionIntensity);
        }
    }

    // Метод для быстрого включения/выключения всех эффектов
    public void ToggleRetroEffects(bool state)
    {
        enableDynamicNoise = state;
        enableDynamicScanlines = state;
        enableColorShift = state;
        enableDistortion = state;
    }

    // Метод для установки интенсивности всех эффектов
    public void SetEffectIntensity(float intensity)
    {
        noiseIntensity = intensity;
        scanlineIntensity = intensity;
        colorShiftIntensity = intensity;
        distortionIntensity = intensity;
    }
}