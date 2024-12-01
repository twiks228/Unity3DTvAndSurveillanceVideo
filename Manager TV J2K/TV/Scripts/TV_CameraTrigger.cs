using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TV_CameraTrigger : MonoBehaviour
{
    [Header("TV Screen Setup")]
    public Renderer tvScreenRenderer; // Renderer ������ ����������

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
    private Material cameraMaterial; // �������� ��� ����������� ������

    private void Start()
    {
        // �������������� ����� �����������
        if (cameraController == null)
            cameraController = GetComponentInParent<TV_CameraController>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // ��������� ������������ �������� ������
        if (tvScreenRenderer != null)
        {
            originalTVScreenMaterial = tvScreenRenderer.material;
        }

        // ��������� UI
        InitializeUI();
    }

    private void InitializeUI()
    {
        // �������� UI ������ ��� ������
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(false);

        // �������� ����� ��������������
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

        // ���� � ���� ��������������
        if (distance <= interactionDistance && !isPlayerInTrigger)
        {
            isPlayerInTrigger = true;
            ShowInteractionText(true);
        }
        // ����� �� ���� ��������������
        else if (distance > interactionDistance && isPlayerInTrigger)
        {
            isPlayerInTrigger = false; // ���������� �����
            ShowInteractionText(false);
            ExitCameraView();
        }
    }

    private void HandleCameraInteraction()
    {
        // ��������� ������� ������� E
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
        // ��������� ��������� �������
        DisableScripts();

        // ���������� ������ ������
        if (tvScreenCamera != null)
        {
            tvScreenCamera.enabled = true;
        }

        // ���������� ���������� �����
        if (cameraController != null)
        {
            cameraController.ActivateCameraControl();
        }

        // ������ �������� ������ �� ������
        if (tvScreenRenderer != null && tvScreenCamera != null)
        {
            // ������� ����� �������� ��� ����������� ������
            cameraMaterial = new Material(Shader.Find("Unlit/Texture"));
            cameraMaterial.mainTexture = tvScreenCamera.targetTexture;
            tvScreenRenderer.material = cameraMaterial;
        }

        // ���������� UI
        ShowCameraUI(true);

        isCameraViewActive = true;
    }

    private void ExitCameraView()
    {
        // ������������ ������ ������
        if (tvScreenCamera != null)
        {
            tvScreenCamera.enabled = false;
        }

        // ������������ ���������� �����
        if (cameraController != null)
        {
            cameraController.DeactivateCameraControl();
        }

        // ��������������� ��������� ��������
        RestoreScripts();

        // ��������������� ������������ �������� ������
        if (tvScreenRenderer != null)
        {
            tvScreenRenderer.material = originalTVScreenMaterial;
        }

        // �������� UI
        ShowCameraUI(false);

        isCameraViewActive = false;
    }

    private void DisableScripts()
    {
        // ��������� ������������ ��������� ��������
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
        // ��������������� ��������� ��������
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
        // ����������/�������� �������� UI ������
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(show);

        // ��������� ����� ������, ���� �����
        UpdateCameraInfoText();
    }

    private void UpdateCameraInfoText()
    {
        if (cameraInfoText != null && cameraController != null)
        {
            // �������� ���������� � ������� ������
            string cameraName = cameraController.GetCurrentCameraName();
            cameraInfoText.text = cameraName;
        }
    }
}