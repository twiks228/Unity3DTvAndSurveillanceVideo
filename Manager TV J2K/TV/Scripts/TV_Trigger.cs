using UnityEngine;

public class TV_Trigger : MonoBehaviour
{
    [Header("TV Interaction")]
    public TV_VideoControl tvVideoControl;
    public KeyCode interactionKey = KeyCode.E;

    [Header("Sound Settings")]
    public AudioClip tvTurnOnSound;
    public AudioClip tvTurnOffSound;
    public AudioSource audioSource;

    [Header("Sound Settings")]
    [Range(0f, 1f)] public float soundVolume = 1f;

    private bool isPlayerInTrigger = false; // Добавлена эта строка
    private bool isCurrentlyOn = false;

    private void Start()
    {
        // Автоматический поиск, если не назначен
        if (tvVideoControl == null)
        {
            tvVideoControl = GetComponentInParent<TV_VideoControl>();
        }

        // Автоматический поиск AudioSource, если не назначен
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Создание AudioSource, если его нет
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Настройка громкости
        if (audioSource != null)
        {
            audioSource.volume = soundVolume;
        }

        // Проверка наличия компонента
        if (tvVideoControl == null)
        {
            Debug.LogError("No TV_VideoControl found on parent objects!");
        }
    }

    private void Update()
    {
        // Проверка триггера и взаимодействия
        if (isPlayerInTrigger && tvVideoControl != null)
        {
            // Обработка нажатия клавиши
            if (Input.GetKeyDown(interactionKey))
            {
                ToggleTVWithSound();
            }
        }
    }

    private void ToggleTVWithSound()
    {
        // Переключаем телевизор
        tvVideoControl.ToggleTV();

        // Воспроизводим звук
        if (audioSource != null)
        {
            // Выбираем звук в зависимости от текущего состояния
            AudioClip soundToPlay = isCurrentlyOn ? tvTurnOffSound : tvTurnOnSound;

            if (soundToPlay != null)
            {
                audioSource.clip = soundToPlay;
                audioSource.Play();
            }

            // Инвертируем состояние
            isCurrentlyOn = !isCurrentlyOn;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверка входа игрока в триггер
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            // Показать текст взаимодействия ТОЛЬКО в триггере
            if (tvVideoControl != null && tvVideoControl.interactionText != null)
            {
                tvVideoControl.interactionText.gameObject.SetActive(true);
                tvVideoControl.interactionText.text = isCurrentlyOn
                    ? "Press E to turn off TV"
                    : "Press E to turn on TV";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Выход игрока из триггера
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            // Полное скрытие текста при выходе из триггера
            if (tvVideoControl != null && tvVideoControl.interactionText != null)
            {
                tvVideoControl.interactionText.gameObject.SetActive(false);
            }
        }
    }
}
