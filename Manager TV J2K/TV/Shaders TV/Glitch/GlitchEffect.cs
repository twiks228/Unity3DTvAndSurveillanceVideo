using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    [Header("Glitch Material")]
    public Material glitchMaterial;

    [Header("Glitch Intensity")]
    [Range(0f, 1f)] public float glitchIntensity = 0.1f;
    [Range(0f, 0.1f)] public float colorSplitIntensity = 0.02f;

    [Header("Glitch Timing")]
    public float minGlitchInterval = 1f;
    public float maxGlitchInterval = 5f;
    public float glitchDuration = 0.5f;

    [Header("Block Settings")]
    [Range(1, 50)] public float blockSize = 10f;

    [Header("Advanced Settings")]
    public bool randomizeGlitch = true;
    public AnimationCurve glitchIntensityCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float nextGlitchTime;
    private float glitchEndTime;
    private bool isGlitching = false;

    private void Start()
    {
        ResetGlitchTimer();
    }

    private void Update()
    {
        if (!glitchMaterial) return;

        // Управление глитч-эффектом
        ManageGlitchTiming();

        // Обновление параметров шейдера
        UpdateShaderProperties();
    }

    private void ManageGlitchTiming()
    {
        // Проверка времени для глитча
        if (Time.time >= nextGlitchTime)
        {
            StartGlitch();
        }

        // Завершение глитча
        if (isGlitching && Time.time >= glitchEndTime)
        {
            EndGlitch();
        }
    }

    private void StartGlitch()
    {
        isGlitching = true;
        glitchEndTime = Time.time + glitchDuration;

        // Рандомизация интенсивности, если включена
        if (randomizeGlitch)
        {
            glitchIntensity = Random.Range(0.1f, 0.5f);
            colorSplitIntensity = Random.Range(0.01f, 0.05f);
            blockSize = Random.Range(5f, 30f);
        }
    }

    private void EndGlitch()
    {
        isGlitching = false;
        ResetGlitchTimer();

        // Сброс параметров
        glitchIntensity = 0f;
        colorSplitIntensity = 0f;
    }

    private void ResetGlitchTimer()
    {
        nextGlitchTime = Time.time + Random.Range(minGlitchInterval, maxGlitchInterval);
    }

    private void UpdateShaderProperties()
    {
        // Динамическое обновление параметров шейдера
        float currentIntensity = isGlitching
            ? glitchIntensityCurve.Evaluate(1 - (glitchEndTime - Time.time) / glitchDuration) * glitchIntensity
            : 0f;

        glitchMaterial.SetFloat("_GlitchIntensity", currentIntensity);
        glitchMaterial.SetFloat("_ColorSplitIntensity", colorSplitIntensity);
        glitchMaterial.SetFloat("_BlockSize", blockSize);
    }

    // Публичные методы для управления
    public void SetGlitchIntensity(float intensity)
    {
        glitchIntensity = Mathf.Clamp01(intensity);
    }

    public void TriggerManualGlitch()
    {
        StartGlitch();
    }
}