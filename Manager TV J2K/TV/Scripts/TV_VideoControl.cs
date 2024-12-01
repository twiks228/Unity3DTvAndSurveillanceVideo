using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using TMPro;

public class TV_VideoControl : MonoBehaviour
{
    [Header("TV Configuration")]
    public GameObject tvModel;
    public Renderer tvScreenRenderer;
    public Material defaultScreenMaterial; // Материал выключенного экрана
    public Material activeScreenMaterial;  // Материал включенного экрана

    [Header("Video Playlist")]
    public List<VideoClip> videoPlaylist = new List<VideoClip>();
    public bool loopPlaylist = true;
    public bool shufflePlaylist = false;

    [Header("UI Interaction")]
    public TextMeshProUGUI interactionText;
    public string onText = "Press E to turn on TV";
    public string offText = "Press E to turn off TV";

    [Header("Video Player Settings")]
    public VideoPlayer videoPlayer;
    [Range(0, 1)] public float volume = 0.5f;

    [Header("Render Texture")]
    public RenderTexture renderTexture;

    [Header("Audio Settings")]
    public AudioSource videoAudioSource;
    public bool muteVideoSound = false;

    private int currentVideoIndex = 0;
    private bool isTVOn = false;

    private void Start()
    {
        InitializeVideoPlayer();
        SetupRenderTexture();
        SetupScreenRenderer();
        DiagnoseAudioIssues();
        // Начальное состояние - экран выключен
        SetScreenState(false);
    }

    private void InitializeVideoPlayer()
    {
        if (videoPlayer == null)
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource; // Важная строка

        // Привязка аудио источника
        if (videoAudioSource == null)
        {
            videoAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Установка громкости
        videoAudioSource.volume = volume;

        // Привязка аудио источника к VideoPlayer
        videoPlayer.SetTargetAudioSource(0, videoAudioSource);
    }

    private void SetupRenderTexture()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1920, 1080, 24);
        }
        videoPlayer.targetTexture = renderTexture;
    }

    private void SetupScreenRenderer()
    {
        if (tvScreenRenderer == null && tvModel != null)
            tvScreenRenderer = tvModel.GetComponentInChildren<Renderer>();
    }

    // Метод для управления состоянием экрана
    private void SetScreenState(bool isActive)
    {
        if (tvScreenRenderer != null)
        {
            // Переключение материала
            tvScreenRenderer.material = isActive ? activeScreenMaterial : defaultScreenMaterial;

            // Очистка Render Texture при выключении
            if (!isActive && renderTexture != null)
            {
                RenderTexture.active = renderTexture;
                GL.Clear(true, true, Color.black);
                RenderTexture.active = null;
            }
        }
    }
    public void ToggleSound()
    {
        // Переключение режима звука
        muteVideoSound = !muteVideoSound;
        UpdateVideoSound();
    }
    private void UpdateVideoSound()
    {
        if (videoPlayer == null || videoAudioSource == null) return;

        // Применение настроек звука
        videoAudioSource.mute = muteVideoSound;

        // Если видео играет, применяем настройки звука
        if (videoPlayer.isPlaying)
        {
            if (muteVideoSound)
            {
                videoAudioSource.Pause();
            }
            else
            {
                videoAudioSource.UnPause();
                videoAudioSource.volume = volume; // Восстановление громкости
            }
        }
    }

    public void ToggleTV()
    {
        isTVOn = !isTVOn;

        if (isTVOn)
        {
            TurnOnTV();
        }
        else
        {
            TurnOffTV();
        }
    }

    public void TurnOnTV()
    {
        isTVOn = true;
        SetScreenState(true);
        PlayFirstVideo();
        UpdateVideoSound(); // Добавьте эту строку
    }

  

    public void TurnOffTV()
    {
        isTVOn = false;
        StopVideo();
        SetScreenState(false);
    }

    public void PlayFirstVideo()
    {
        if (videoPlaylist.Count > 0)
        {
            currentVideoIndex = shufflePlaylist
                ? Random.Range(0, videoPlaylist.Count)
                : 0;
            PlayCurrentVideo();
        }
    }

    public void PlayCurrentVideo()
    {
        if (videoPlaylist.Count == 0 || !isTVOn) return;

        VideoClip currentClip = videoPlaylist[currentVideoIndex];

        if (currentClip != null)
        {
            videoPlayer.clip = currentClip;
            videoPlayer.Play();
            UpdateVideoSound(); // Добавьте эту строку
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        PlayNextVideo();
    }

    public void PlayNextVideo()
    {
        if (videoPlaylist.Count == 0 || !isTVOn) return;

        currentVideoIndex++;

        if (currentVideoIndex >= videoPlaylist.Count)
        {
            if (loopPlaylist)
            {
                currentVideoIndex = 0;
            }
            else
            {
                TurnOffTV();
                return;
            }
        }

        PlayCurrentVideo();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }
    private void DiagnoseAudioIssues()
    {
        if (videoPlayer == null)
            Debug.LogError("VideoPlayer is null");

        if (videoAudioSource == null)
            Debug.LogError("VideoAudioSource is null");

        if (videoPlaylist.Count == 0)
            Debug.LogError("Video Playlist is empty");

        if (videoPlayer != null)
        {
            Debug.Log($"Current Video Clip: {videoPlayer.clip}");
            Debug.Log($"Is Playing: {videoPlayer.isPlaying}");
        }

        if (videoAudioSource != null)
        {
            Debug.Log($"Audio Mute: {videoAudioSource.mute}");
            Debug.Log($"Audio Volume: {videoAudioSource.volume}");
        }
    }
}