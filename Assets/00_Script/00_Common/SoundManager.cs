using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    public enum SfxId
    {
        GameStart,
        ItemPickUp,
        ItemDrop,
        KeyUse,
        DoorOpen,
        HoleEnter,
        PlayerWalk,
        PlayerRun,
        PlayerExhausted,
        HutakuchionnaBreath,
        HutakuchionnaDetection,
        EnemyDie,
        Heartbeat,
        OldWoodDoorOpen,
        Spike,
    }
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("3D SFX Pool")]
    [SerializeField, Min(1)] private int initial3DPoolSize = 8;
    [SerializeField] private float default3DMinDistance = 1f;
    [SerializeField] private float default3DMaxDistance = 20f;

    [Header("World SFX Ranges")]
    [SerializeField] private float enemyDoorMinDistance = 2f;
    [SerializeField] private float enemyDoorMaxDistance = 40f;

    [Header("Footstep")]
    [SerializeField] private float walkFootstepInterval = 0.45f;
    [SerializeField] private float runFootstepInterval = 0.30f;
    [SerializeField] private float footstepMinDistance = 1f;
    [SerializeField] private float footstepMaxDistance = 12f;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Scene Audio")]
    [SerializeField] private AudioClip titleMainBgm;
    [SerializeField] private AudioClip natureAmbientLoop;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip gameStartSfx;
    [SerializeField] private AudioClip itemPickUpSfx;
    [SerializeField] private AudioClip itemDropSfx;
    [SerializeField] private AudioClip keyUseSfx;
    [SerializeField] private AudioClip doorOpenSfx;
    [SerializeField] private AudioClip holeEnterSfx;
    [SerializeField] private AudioClip oldWoodDoorOpenSfx;
    [SerializeField] private AudioClip spikeSfx;

    [Header("Player Sounds")]
    [SerializeField] private AudioClip playerWalkSfx;
    [SerializeField] private AudioClip playerRunSfx;
    [SerializeField] private AudioClip playerExhaustedSfx;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip hutakuchionna_breath;
    [SerializeField] private AudioClip hutakuchionna_chaseBGM;
    [SerializeField] private AudioClip hutakuchionna_detection;
    [SerializeField] private AudioClip enemyDieSfx;
    [SerializeField] private AudioClip heartbeatSfx;

    private float defaultBgmVolume;
    private float defaultAmbientVolume;
    private float defaultSfxVolume;

    private bool heartbeatRequested;
    private Transform pooled3DRoot;
    private readonly Queue<AudioSource> available3DSources = new Queue<AudioSource>();
    private readonly HashSet<AudioSource> active3DSources = new HashSet<AudioSource>();
    private readonly Dictionary<int, float> footstepNextPlayTimeByTargetId = new Dictionary<int, float>();
    private readonly Dictionary<int, HashSet<AudioSource>> activeFootstepSourcesByTargetId = new Dictionary<int, HashSet<AudioSource>>();
    private readonly Dictionary<AudioSource, int> footstepOwnerBySource = new Dictionary<AudioSource, int>();
    private float nextPlayerExhaustedPlayableTime;

    protected override void Awake()
    {
        IsDontDestroyOnLoad = true;
        base.Awake();

        if (Instance != this)
            return;

        if (bgmSource == null)
            bgmSource = gameObject.AddComponent<AudioSource>();

        if (ambientSource == null)
            ambientSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (heartbeatSource == null)
            heartbeatSource = gameObject.AddComponent<AudioSource>();

        ConfigureBgmSource(bgmSource);
        ConfigureAmbientSource(ambientSource);
        ConfigureSfxSource(sfxSource);
        ConfigureSfxSource(heartbeatSource);

        defaultBgmVolume = Mathf.Clamp01(bgmVolume);
        defaultAmbientVolume = Mathf.Clamp01(ambientVolume);
        defaultSfxVolume = Mathf.Clamp01(sfxVolume);

        Initialize3DPool();

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        ApplySceneAudioPolicy(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    public void PlayGameStartSfx()
    {
        PlayUISFX(SfxId.GameStart);
    }

    private void ConfigureBgmSource(AudioSource source)
    {
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
    }

    private void ConfigureSfxSource(AudioSource source)
    {
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
    }

    private void ConfigureAmbientSource(AudioSource source)
    {
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneAudioPolicy(scene.name);
    }

    private void ApplySceneAudioPolicy(string sceneName)
    {
        bool isTitleScene = sceneName == "Title";
        bool isIngameOrTutorial = sceneName == "InGame" || sceneName == "Tutorial";

        if (isTitleScene)
        {
            RestoreDefaultVolumes();
            PlayBGM(titleMainBgm);
            PlayAmbient(natureAmbientLoop, true);
            return;
        }

        if (isIngameOrTutorial)
        {
            RestoreDefaultVolumes();
            StopBGM();
            PlayAmbient(natureAmbientLoop, true);
            return;
        }

        StopBGM();
        StopAmbient();
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
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // BGM 정지
    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    public void PlayAmbient(AudioClip clip, bool restartFromStart = false)
    {
        if (ambientSource == null || clip == null) return;
        if (ambientSource.clip == clip && ambientSource.isPlaying && !restartFromStart) return;

        ambientSource.clip = clip;
        ambientSource.volume = ambientVolume;
        ambientSource.loop = true;

        if (restartFromStart)
            ambientSource.time = 0f;

        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource == null) return;
        ambientSource.Stop();
    }

    // 2D 효과음 재생 (UI/피드백용)
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // 기존 호환: 전달된 AudioSource로 3D 효과음 재생
    public void PlaySFX3D(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.spatialBlend = 1f;
        audioSource.spread = 0f;
        audioSource.dopplerLevel = 0f;
        audioSource.panStereo = 0f;
        if (audioSource.minDistance <= 0f) audioSource.minDistance = default3DMinDistance;
        if (audioSource.maxDistance <= audioSource.minDistance) audioSource.maxDistance = default3DMaxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.PlayOneShot(clip, sfxVolume);
    }

    // SfxId 기반 2D 재생
    public void PlayUISFX(SfxId id)
    {
        PlaySFX(GetSfx(id));
    }

    public void PlayEnemyChaseBGM()
    {
        PlayBGM(hutakuchionna_chaseBGM);
    }

    public void PlayEnemyDetectionCue()
    {
        PlayUISFX(SfxId.HutakuchionnaDetection);
    }

    public void PlayEnemyDieCue()
    {
        PlayUISFX(SfxId.EnemyDie);
    }

    public void PlayItemPickUpCue()
    {
        PlayUISFX(SfxId.ItemPickUp);
    }

    public void PlayItemDropCue()
    {
        PlayUISFX(SfxId.ItemDrop);
    }

    public void PlayKeyUseCue()
    {
        PlayUISFX(SfxId.KeyUse);
    }

    public void PlaySpikeCue()
    {
        PlayUISFX(SfxId.Spike);
    }

    public void PlaySpikeAt(Vector3 worldPosition, float minDistance = -1f, float maxDistance = -1f)
    {
        PlayWorldSFXAt(SfxId.Spike, worldPosition, minDistance, maxDistance);
    }

    public void PlayDoorOpenAt(Vector3 worldPosition, bool openedByEnemy = false)
    {
        // 2D 게임 기준으로 문 소리는 XY 거리만 반영하고 Z 차이는 무시한다.
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            worldPosition.z = listener.transform.position.z;
        }

        if (openedByEnemy)
        {
            PlayWorldSFXAt(SfxId.DoorOpen, worldPosition, enemyDoorMinDistance, enemyDoorMaxDistance);
            return;
        }

        PlayWorldSFXAt(SfxId.DoorOpen, worldPosition);
    }

    public void PlayOldWoodDoorOpenAt(Vector3 worldPosition)
    {
        AudioClip clip = GetSfx(SfxId.OldWoodDoorOpen);
        if (clip == null || sfxSource == null)
            return;

        // 하프미러 문 소리는 거리 감쇠 없이 전체에서 들리도록 2D로 재생
        sfxSource.PlayOneShot(clip, sfxVolume * 0.5f);
    }

    public void PlayPlayerExhausted()
    {
        AudioClip clip = GetSfx(SfxId.PlayerExhausted);
        if (clip == null || sfxSource == null)
            return;

        // 같은 exhausted 사운드가 재생 중인 시간대에는 중복 호출을 막는다.
        if (Time.time < nextPlayerExhaustedPlayableTime)
            return;

        PlaySFX(clip);
        nextPlayerExhaustedPlayableTime = Time.time + clip.length;
    }

    public void RequestPlayerFootstep(Transform target, bool isRun)
    {
        if (target == null) return;

        SfxId footstepId = isRun ? SfxId.PlayerRun : SfxId.PlayerWalk;
        AudioClip clip = GetSfx(footstepId);
        if (clip == null) return;

        int key = target.GetInstanceID();
        float interval = isRun ? runFootstepInterval : walkFootstepInterval;
        if (interval <= 0f) interval = 0.1f;

        float minIntervalByClip = Mathf.Max(0.05f, clip.length * 0.95f);
        float resolvedInterval = Mathf.Max(interval, minIntervalByClip);

        if (footstepNextPlayTimeByTargetId.TryGetValue(key, out float nextTime) && Time.time < nextTime)
            return;

        AudioSource source = Rent3DSource();
        Configure3DSource(source, footstepMinDistance, footstepMaxDistance);
        source.transform.position = target.position;

        FollowTarget follow = source.GetComponent<FollowTarget>();
        if (follow != null)
        {
            follow.Target = null;
            follow.enabled = false;
        }

        source.PlayOneShot(clip, sfxVolume);
        RegisterFootstepSource(key, source);
        StartCoroutine(Return3DSourceRoutine(source, clip.length));

        footstepNextPlayTimeByTargetId[key] = Time.time + resolvedInterval;
    }

    public void ClearPlayerFootstep(Transform target)
    {
        if (target == null) return;

        int key = target.GetInstanceID();
        footstepNextPlayTimeByTargetId.Remove(key);

        if (!activeFootstepSourcesByTargetId.TryGetValue(key, out HashSet<AudioSource> sources) || sources == null)
            return;

        AudioSource[] sourceArray = new AudioSource[sources.Count];
        sources.CopyTo(sourceArray);
        for (int i = 0; i < sourceArray.Length; i++)
        {
            Return3DSource(sourceArray[i]);
        }

        activeFootstepSourcesByTargetId.Remove(key);
    }

    // SfxId 기반 3D 재생 (고정 위치)
    public void PlayWorldSFXAt(SfxId id, Vector3 worldPosition, float minDistance = -1f, float maxDistance = -1f)
    {
        AudioClip clip = GetSfx(id);
        if (clip == null) return;

        AudioSource source = Rent3DSource();
        Configure3DSource(source, minDistance, maxDistance);
        source.transform.position = worldPosition;

        FollowTarget follow = source.GetComponent<FollowTarget>();
        if (follow != null)
        {
            follow.Target = null;
            follow.enabled = false;
        }

        source.PlayOneShot(clip, sfxVolume);
        StartCoroutine(Return3DSourceRoutine(source, clip.length));
    }

    // SfxId 기반 3D 재생 (이동 대상 추적)
    public void PlayWorldSFXFollow(SfxId id, Transform target, float minDistance = -1f, float maxDistance = -1f)
    {
        if (target == null) return;

        AudioClip clip = GetSfx(id);
        if (clip == null) return;

        AudioSource source = Rent3DSource();
        Configure3DSource(source, minDistance, maxDistance);
        source.transform.position = target.position;

        FollowTarget follow = source.GetComponent<FollowTarget>();
        if (follow == null)
            follow = source.gameObject.AddComponent<FollowTarget>();
        follow.Target = target;
        follow.enabled = true;

        source.PlayOneShot(clip, sfxVolume);
        StartCoroutine(Return3DSourceRoutine(source, clip.length));
    }

    public AudioClip GetSfx(SfxId id)
    {
        switch (id)
        {
            case SfxId.GameStart: return gameStartSfx;
            case SfxId.ItemPickUp: return itemPickUpSfx;
            case SfxId.ItemDrop: return itemDropSfx;
            case SfxId.KeyUse: return keyUseSfx;
            case SfxId.DoorOpen: return doorOpenSfx;
            case SfxId.OldWoodDoorOpen: return oldWoodDoorOpenSfx;
            case SfxId.HoleEnter: return holeEnterSfx;
            case SfxId.PlayerWalk: return playerWalkSfx;
            case SfxId.PlayerRun: return playerRunSfx;
            case SfxId.PlayerExhausted: return playerExhaustedSfx;
            case SfxId.HutakuchionnaBreath: return hutakuchionna_breath;
            case SfxId.HutakuchionnaDetection: return hutakuchionna_detection;
            case SfxId.EnemyDie: return enemyDieSfx;
            case SfxId.Heartbeat: return heartbeatSfx;
            case SfxId.Spike: return spikeSfx;

            default: return null;
        }
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
        ambientVolume = bgmVolume;
        if (ambientSource != null)
            ambientSource.volume = ambientVolume;
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void RestoreDefaultVolumes()
    {
        bgmVolume = defaultBgmVolume;
        ambientVolume = defaultAmbientVolume;
        sfxVolume = defaultSfxVolume;

        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        if (ambientSource != null)
            ambientSource.volume = ambientVolume;

        if (heartbeatSource != null && heartbeatSource.isPlaying)
            heartbeatSource.volume = sfxVolume;
    }

    private void Initialize3DPool()
    {
        GameObject rootObject = new GameObject("Pooled3D_SFX");
        rootObject.transform.SetParent(transform, false);
        pooled3DRoot = rootObject.transform;

        for (int i = 0; i < initial3DPoolSize; i++)
        {
            AudioSource source = CreatePooled3DSource(i);
            available3DSources.Enqueue(source);
        }
    }

    private AudioSource CreatePooled3DSource(int index)
    {
        GameObject sourceObject = new GameObject($"SFX3D_{index}");
        sourceObject.transform.SetParent(pooled3DRoot, false);

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.spread = 0f;
        source.dopplerLevel = 0f;
        source.panStereo = 0f;
        source.minDistance = default3DMinDistance;
        source.maxDistance = default3DMaxDistance;

        FollowTarget follow = sourceObject.AddComponent<FollowTarget>();
        follow.enabled = false;

        return source;
    }

    private AudioSource Rent3DSource()
    {
        AudioSource source;

        if (available3DSources.Count > 0)
        {
            source = available3DSources.Dequeue();
        }
        else
        {
            source = CreatePooled3DSource(active3DSources.Count + available3DSources.Count);
        }

        active3DSources.Add(source);
        return source;
    }

    private void Configure3DSource(AudioSource source, float minDistance, float maxDistance)
    {
        if (source == null) return;

        float resolvedMin = minDistance > 0f ? minDistance : default3DMinDistance;
        float resolvedMax = maxDistance > 0f ? maxDistance : default3DMaxDistance;
        if (resolvedMax <= resolvedMin)
            resolvedMax = resolvedMin + 0.1f;

        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = resolvedMin;
        source.maxDistance = resolvedMax;
        source.loop = false;
        source.playOnAwake = false;
    }

    private IEnumerator Return3DSourceRoutine(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.05f);
        Return3DSource(source);
    }

    private void Return3DSource(AudioSource source)
    {
        if (source == null) return;
        if (!active3DSources.Contains(source)) return;

        UnregisterFootstepSource(source);

        source.Stop();

        FollowTarget follow = source.GetComponent<FollowTarget>();
        if (follow != null)
        {
            follow.Target = null;
            follow.enabled = false;
        }

        source.transform.SetParent(pooled3DRoot, false);
        source.transform.localPosition = Vector3.zero;

        active3DSources.Remove(source);
        available3DSources.Enqueue(source);
    }

    private void RegisterFootstepSource(int ownerTargetId, AudioSource source)
    {
        if (source == null) return;

        if (!activeFootstepSourcesByTargetId.TryGetValue(ownerTargetId, out HashSet<AudioSource> sources) || sources == null)
        {
            sources = new HashSet<AudioSource>();
            activeFootstepSourcesByTargetId[ownerTargetId] = sources;
        }

        sources.Add(source);
        footstepOwnerBySource[source] = ownerTargetId;
    }

    private void UnregisterFootstepSource(AudioSource source)
    {
        if (source == null) return;
        if (!footstepOwnerBySource.TryGetValue(source, out int ownerTargetId)) return;

        if (activeFootstepSourcesByTargetId.TryGetValue(ownerTargetId, out HashSet<AudioSource> sources) && sources != null)
        {
            sources.Remove(source);
            if (sources.Count == 0)
                activeFootstepSourcesByTargetId.Remove(ownerTargetId);
        }

        footstepOwnerBySource.Remove(source);
    }

    private sealed class FollowTarget : MonoBehaviour
    {
        public Transform Target;

        private void LateUpdate()
        {
            if (Target == null) return;
            transform.position = Target.position;
        }
    }

    // Getter 메서드들
    public AudioClip GetGameStartSfx() => gameStartSfx;
    public AudioClip GetItemPickUpSfx() => itemPickUpSfx;
    public AudioClip GetItemDropSfx() => itemDropSfx;
    public AudioClip GetKeyUseSfx() => keyUseSfx;
    public AudioClip GetDoorOpenSfx() => doorOpenSfx;
    public AudioClip GetOldWoodDoorOpenSfx() => oldWoodDoorOpenSfx;
    public AudioClip GetHoleEnterSfx() => holeEnterSfx;
    public AudioClip GetPlayerWalkSfx() => playerWalkSfx;
    public AudioClip GetPlayerRunSfx() => playerRunSfx;
    public AudioClip GetPlayerExhaustedSfx() => playerExhaustedSfx;
    public AudioClip GetHutakuchionna_Breath() => hutakuchionna_breath;
    public AudioClip GetHutakuchionna_ChaseBGM() => hutakuchionna_chaseBGM;
    public AudioClip GetHutakuchionna_Detection() => hutakuchionna_detection;
    public AudioClip GetEnemyDieSfx() => enemyDieSfx;
    public AudioClip GetHeartbeatSfx() => heartbeatSfx;
    public AudioClip GetSpikeSfx() => spikeSfx;
}
