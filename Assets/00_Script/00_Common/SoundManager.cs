using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip gameStartSfx;
    [SerializeField] private AudioClip itemPickUpSfx;
    [SerializeField] private AudioClip itemDropSfx;
    [SerializeField] private AudioClip keyUseSfx;
    [SerializeField] private AudioClip doorOpenSfx;
    [SerializeField] private AudioClip holeEnterSfx;

    [Header("Player Sounds")]
    [SerializeField] private AudioClip playerWalkSfx;
    [SerializeField] private AudioClip playerRunSfx;
    [SerializeField] private AudioClip playerExhaustedSfx;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip hutakuchionna_breath;
    [SerializeField] private AudioClip hutakuchionna_chaseBGM;
    [SerializeField] private AudioClip hutakuchionna_detection;
    [SerializeField] private AudioClip heartbeatSfx;

    private bool heartbeatRequested;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (heartbeatSource == null)
            {
                heartbeatSource = gameObject.AddComponent<AudioSource>();
                heartbeatSource.playOnAwake = false;
                heartbeatSource.loop = false;
                heartbeatSource.spatialBlend = 0f;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGameStartSfx()
    {
        PlaySFX(gameStartSfx);
    }

    private void LateUpdate()
    {
        if (!heartbeatRequested && heartbeatSource != null && heartbeatSource.isPlaying)
        {
            heartbeatSource.Stop();
        }

        heartbeatRequested = false;
    }

    // BGM 재생
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // BGM 정지
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // 3D 위치에서 효과음 재생
    public void PlaySFX3D(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip, sfxVolume);
    }

    public void RequestHeartbeat()
    {
        if (heartbeatSource == null || heartbeatSfx == null)
            return;

        heartbeatRequested = true;
        heartbeatSource.volume = sfxVolume;

        if (!heartbeatSource.isPlaying)
        {
            heartbeatSource.clip = heartbeatSfx;
            heartbeatSource.Play();
        }
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    // Getter 메서드들
    public AudioClip GetGameStartSfx() => gameStartSfx;
    public AudioClip GetItemPickUpSfx() => itemPickUpSfx;
    public AudioClip GetItemDropSfx() => itemDropSfx;
    public AudioClip GetKeyUseSfx() => keyUseSfx;
    public AudioClip GetDoorOpenSfx() => doorOpenSfx;
    public AudioClip GetHoleEnterSfx() => holeEnterSfx;
    public AudioClip GetPlayerWalkSfx() => playerWalkSfx;
    public AudioClip GetPlayerRunSfx() => playerRunSfx;
    public AudioClip GetPlayerExhaustedSfx() => playerExhaustedSfx;
    public AudioClip GetHutakuchionna_Breath() => hutakuchionna_breath;
    public AudioClip GetHutakuchionna_ChaseBGM() => hutakuchionna_chaseBGM;
    public AudioClip GetHutakuchionna_Detection() => hutakuchionna_detection;
    public AudioClip GetHeartbeatSfx() => heartbeatSfx;
}
