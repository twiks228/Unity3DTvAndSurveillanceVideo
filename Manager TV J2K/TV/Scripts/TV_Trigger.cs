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

    private bool isPlayerInTrigger = false; // ��������� ��� ������
    private bool isCurrentlyOn = false;

    private void Start()
    {
        // �������������� �����, ���� �� ��������
        if (tvVideoControl == null)
        {
            tvVideoControl = GetComponentInParent<TV_VideoControl>();
        }

        // �������������� ����� AudioSource, ���� �� ��������
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // �������� AudioSource, ���� ��� ���
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // ��������� ���������
        if (audioSource != null)
        {
            audioSource.volume = soundVolume;
        }

        // �������� ������� ����������
        if (tvVideoControl == null)
        {
            Debug.LogError("No TV_VideoControl found on parent objects!");
        }
    }

    private void Update()
    {
        // �������� �������� � ��������������
        if (isPlayerInTrigger && tvVideoControl != null)
        {
            // ��������� ������� �������
            if (Input.GetKeyDown(interactionKey))
            {
                ToggleTVWithSound();
            }
        }
    }

    private void ToggleTVWithSound()
    {
        // ����������� ���������
        tvVideoControl.ToggleTV();

        // ������������� ����
        if (audioSource != null)
        {
            // �������� ���� � ����������� �� �������� ���������
            AudioClip soundToPlay = isCurrentlyOn ? tvTurnOffSound : tvTurnOnSound;

            if (soundToPlay != null)
            {
                audioSource.clip = soundToPlay;
                audioSource.Play();
            }

            // ����������� ���������
            isCurrentlyOn = !isCurrentlyOn;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �������� ����� ������ � �������
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            // �������� ����� �������������� ������ � ��������
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
        // ����� ������ �� ��������
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            // ������ ������� ������ ��� ������ �� ��������
            if (tvVideoControl != null && tvVideoControl.interactionText != null)
            {
                tvVideoControl.interactionText.gameObject.SetActive(false);
            }
        }
    }
}