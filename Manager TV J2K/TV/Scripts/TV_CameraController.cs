using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TV_CameraController : MonoBehaviour
{
    [Header("Camera Setup")]
    public List<Camera> securityCameras = new List<Camera>();
    public Camera currentActiveCamera;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    public float maxYRotation = 180f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 30f;
    public float maxZoom = 90f;

    [Header("UI")]
    public TextMeshProUGUI cameraNameText;

    private bool isCameraControlActive = false;
    private int currentCameraIndex = 0;
    private float currentYRotation = 0f;
    private float currentZoom = 60f;
    private Quaternion initialRotation;

    private void Start()
    {
        InitializeCameras();
    }

    private void InitializeCameras()
    {
        for (int i = 0; i < securityCameras.Count; i++)
        {
            if (securityCameras[i] != null)
            {
                if (securityCameras[i].targetTexture == null)
                {
                    RenderTexture newRenderTexture = new RenderTexture(1920, 1080, 24);
                    securityCameras[i].targetTexture = newRenderTexture;
                }

                securityCameras[i].enabled = false;
            }
        }
    }

    private void Update()
    {
        if (isCameraControlActive)
        {
            HandleCameraControls();
        }
    }

    private void HandleCameraControls()
    {
        if (currentActiveCamera == null) return;

        // Переключение камер
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchToNextCamera();
            return;
        }

        // Поворот влево/вправо
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.A))
            rotationInput = -1f;
        if (Input.GetKey(KeyCode.D))
            rotationInput = 1f;

        // Обновление поворота
        currentYRotation += rotationInput * rotationSpeed * Time.deltaTime;
        currentYRotation = Mathf.Clamp(currentYRotation, -maxYRotation, maxYRotation);

        // Зум
        float zoomInput = Input.mouseScrollDelta.y;
        currentZoom -= zoomInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Применение трансформаций
        ApplyCameraTransformations();
    }

    private void ApplyCameraTransformations()
    {
        if (currentActiveCamera == null) return;

        // Сброс позиции
        currentActiveCamera.transform.localPosition = Vector3.zero;

        // Применение поворота с сохранением начальной ориентации
        Quaternion rotationY = Quaternion.Euler(0f, currentYRotation, 0f);
        currentActiveCamera.transform.localRotation = initialRotation * rotationY;

        // Применение зума
        currentActiveCamera.fieldOfView = currentZoom;
    }

    private void SwitchToNextCamera()
    {
        if (securityCameras.Count == 0) return;

        // Деактивируем текущую камеру
        if (currentActiveCamera != null)
            currentActiveCamera.enabled = false;

        // Переключаемся на следующую
        currentCameraIndex = (currentCameraIndex + 1) % securityCameras.Count;
        currentActiveCamera = securityCameras[currentCameraIndex];

        // Активируем новую камеру
        if (currentActiveCamera != null)
        {
            currentActiveCamera.enabled = true;

            // Сохраняем начальную ориентацию при переключении
            initialRotation = currentActiveCamera.transform.localRotation;

            ResetCameraSettings();
        }

        UpdateCameraNameUI();
    }

    private void ResetCameraSettings()
    {
        currentYRotation = 0f;
        currentZoom = 60f;

        if (currentActiveCamera != null)
        {
            currentActiveCamera.transform.localPosition = Vector3.zero;
            currentActiveCamera.transform.localRotation = initialRotation;
        }
    }

    private void UpdateCameraNameUI()
    {
        if (cameraNameText != null && currentActiveCamera != null)
        {
            cameraNameText.text = $"Camera {currentCameraIndex + 1}";
        }
    }

    public void ActivateCameraControl()
    {
        isCameraControlActive = true;

        if (securityCameras.Count > 0)
        {
            SwitchToNextCamera();
        }
    }

    public void DeactivateCameraControl()
    {
        isCameraControlActive = false;

        foreach (var camera in securityCameras)
        {
            if (camera != null)
                camera.enabled = false;
        }
    }

    public string GetCurrentCameraName()
    {
        if (currentActiveCamera != null && cameraNameText != null)
        {
            return cameraNameText.text;
        }
        return "No Camera Selected";
    }
}