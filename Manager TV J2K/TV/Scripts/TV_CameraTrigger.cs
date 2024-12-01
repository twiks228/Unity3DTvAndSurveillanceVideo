using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TV_CameraTrigger : MonoBehaviour
{
    [Header("TV Screen Setup")]
    public Renderer tvScreenRenderer; // Renderer экрана телевизора

    [Header("Camera Interaction")]
    public TV_CameraController cameraController;

    [Header("Camera Setup")]
    public Camera tvScreenCamera;

    [Header("UI Elements")]
    public GameObject cameraUIPanel;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI cameraInfoText;
    public string enterText = "Press E to view cameras";
    public string exitText = "Press E to exit camera view";

    [Header("Player Interaction")]
    public float interactionDistance = 1.5f;
    public Transform playerTransform;

    [Header("Scripts to Disable")]
    public MonoBehaviour[] scriptsToDisable;

    [Header("Input")]
    public KeyCode activationKey = KeyCode.E;

    private bool isPlayerInTrigger = false;
    private bool isCameraViewActive = false;
    private bool[] originalScriptStates;
    private Material originalTVScreenMaterial;
    private Material cameraMaterial; // Материал для отображения камеры

    private void Start()
    {
        // Автоматический поиск компонентов
        if (cameraController == null)
            cameraController = GetComponentInParent<TV_CameraController>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Сохраняем оригинальный материал экрана
        if (tvScreenRenderer != null)
        {
            originalTVScreenMaterial = tvScreenRenderer.material;
        }

        // Настройка UI
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Скрываем UI панель при старте
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(false);

        // Скрываем текст взаимодействия
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckPlayerProximity();

        if (isPlayerInTrigger)
        {
            HandleCameraInteraction();
        }
    }

    private void CheckPlayerProximity()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Вход в зону взаимодействия
        if (distance <= interactionDistance && !isPlayerInTrigger)
        {
            isPlayerInTrigger = true;
            ShowInteractionText(true);
        }
        // Выход из зоны взаимодействия
        else if (distance > interactionDistance && isPlayerInTrigger)
        {
            isPlayerInTrigger = false; // Исправлено здесь
            ShowInteractionText(false);
            ExitCameraView();
        }
    }

    private void HandleCameraInteraction()
    {
        // Проверяем нажатие клавиши E
        if (Input.GetKeyDown(activationKey))
        {
            if (!isCameraViewActive)
            {
                StartCameraView();
            }
            else
            {
                ExitCameraView();
            }
        }
    }

    private void StartCameraView()
    {
        // Отключаем указанные скрипты
        DisableScripts();

        // Активируем камеру экрана
        if (tvScreenCamera != null)
        {
            tvScreenCamera.enabled = true;
        }

        // Активируем контроллер камер
        if (cameraController != null)
        {
            cameraController.ActivateCameraControl();
        }

        // Меняем материал экрана на камеру
        if (tvScreenRenderer != null && tvScreenCamera != null)
        {
            // Создаем новый материал для отображения камеры
            cameraMaterial = new Material(Shader.Find("Unlit/Texture"));
            cameraMaterial.mainTexture = tvScreenCamera.targetTexture;
            tvScreenRenderer.material = cameraMaterial;
        }

        // Показываем UI
        ShowCameraUI(true);

        isCameraViewActive = true;
    }

    private void ExitCameraView()
    {
        // Деактивируем камеру экрана
        if (tvScreenCamera != null)
        {
            tvScreenCamera.enabled = false;
        }

        // Деактивируем контроллер камер
        if (cameraController != null)
        {
            cameraController.DeactivateCameraControl();
        }

        // Восстанавливаем состояние скриптов
        RestoreScripts();

        // Восстанавливаем оригинальный материал экрана
        if (tvScreenRenderer != null)
        {
            tvScreenRenderer.material = originalTVScreenMaterial;
        }

        // Скрываем UI
        ShowCameraUI(false);

        isCameraViewActive = false;
    }

    private void DisableScripts()
    {
        // Сохраняем оригинальные состояния скриптов
        if (scriptsToDisable != null && scriptsToDisable.Length > 0)
        {
            originalScriptStates = new bool[scriptsToDisable.Length];

            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null)
                {
                    originalScriptStates[i] = scriptsToDisable[i].enabled;
                    scriptsToDisable[i].enabled = false;
                }
            }
        }
    }

    private void RestoreScripts()
    {
        // Восстанавливаем состояние скриптов
        if (scriptsToDisable != null && originalScriptStates != null)
        {
            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null && i < originalScriptStates.Length)
                {
                    scriptsToDisable[i].enabled = originalScriptStates[i];
                }
            }
        }
    }

    private void ShowInteractionText(bool show)
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(show);

            if (show)
            {
                interactionText.text = isCameraViewActive
                    ? exitText
                    : enterText;
            }
        }
    }

    private void ShowCameraUI(bool show)
    {
        // Показываем/скрываем основную UI панель
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(show);

        // Обновляем текст камеры, если нужно
        UpdateCameraInfoText();
    }

    private void UpdateCameraInfoText()
    {
        if (cameraInfoText != null && cameraController != null)
        {
            // Получаем информацию о текущей камере
            string cameraName = cameraController.GetCurrentCameraName();
            cameraInfoText.text = cameraName;
        }
    }
}