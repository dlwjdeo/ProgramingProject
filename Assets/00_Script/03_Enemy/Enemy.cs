using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float walkMoveSpeed = 2f;
    [SerializeField] private float runMoveSpeed = 3f;
    [SerializeField] private float doorOpenDelay = 0.5f;
    [SerializeField] private GameObject hairPrefab;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    public AudioClip breathClip;

    [Header("Audio Debug")]
    [SerializeField] private float currentAudioVolume = 0f;

    public EnemyVision EnemyVision { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }
    public Animator EnemyAnimator { get; private set; }

    [SerializeField] private EnemyStateType state;
    public EnemyStateType State => state;

    public RoomController CurrentRoom { get; private set; }

    public EnemyMover Mover { get; private set; }
    public float DefaultMoveSpeed => walkMoveSpeed;
    public float WalkMoveSpeed => walkMoveSpeed;
    public float RunMoveSpeed => runMoveSpeed;
    public float DoorOpenDelay => doorOpenDelay;
    public EnemyMoveMode MoveMode { get; private set; } = EnemyMoveMode.Walk;

    public float Direction { get; private set; } = 1f;
    public ChaseTarget CurrentChaseTarget { get; private set; }
    public bool IsDoorPauseActive { get; private set; }

    private Coroutine doorPauseCoroutine;


    private void Awake()
    {
        Mover = GetComponent<EnemyMover>();
        SetMoveMode(EnemyMoveMode.Walk);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        EnemyVision = GetComponentInChildren<EnemyVision>();
        EnemyAnimator = GetComponent<Animator>();
        
        StateMachine = new EnemyStateMachine(this);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && breathClip != null)
        {
            audioSource.clip = breathClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnEnable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedEnemyRoom += SetCurrentRoom;

        if (RoomManager.Instance != null && RoomManager.Instance.EnemyRoom != null)
            SetCurrentRoom(RoomManager.Instance.EnemyRoom);
    }

    private void OnDisable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedEnemyRoom -= SetCurrentRoom;

        IsDoorPauseActive = false;
        if (doorPauseCoroutine != null)
        {
            StopCoroutine(doorPauseCoroutine);
            doorPauseCoroutine = null;
        }
    }

    private void Update()
    {
        StateMachine.Update();
        UpdateAudioVolume();
        UpdateDetectionSounds();
    }

    private void UpdateDetectionSounds()
    {
        if (EnemyVision == null || Player.Instance == null)
        {
            return;
        }

        float xDistance = Mathf.Abs(transform.position.x - Player.Instance.transform.position.x);
        
        if (xDistance < 10f)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.RequestHeartbeat();
        }
    }

    private void UpdateAudioVolume()
    {
        if (audioSource == null || Player.Instance == null) return;

        if (state == EnemyStateType.Chase)
        {
            audioSource.volume = 0f;
            currentAudioVolume = 0f;
            return;
        }

        // 플레이어와의 상대 위치
        Vector2 playerPos = Player.Instance.transform.position;

        // x축만 기준으로 거리 계산
        float xDistance = Mathf.Abs(transform.position.x - playerPos.x);

        // y축 거리
        float yDistance = Mathf.Abs(transform.position.y - playerPos.y);

        // x축 거리에 따른 기본 볼륨
        float maxXDistance = 20f;
        float minXDistance = 1f;

        float baseVolume;
        if (xDistance <= minXDistance)
            baseVolume = 1f;
        else if (xDistance >= maxXDistance)
            baseVolume = 0f;
        else
            baseVolume = Mathf.Lerp(1f, 0f, (xDistance - minXDistance) / (maxXDistance - minXDistance));

        // y축 거리에 따른 보정
        float finalVolume = baseVolume;

        if (yDistance >= 20f)
            finalVolume = 0f;  // y축 20 이상: 안 들림
        else if (yDistance >= 10f)
            finalVolume *= 0.5f;  // y축 10 이상: 절반

        audioSource.volume = finalVolume;
        currentAudioVolume = finalVolume;  // Inspector에서 표시
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerStay2D(collision);
    }

    public void SetChaseTargetPlayer(Transform playerTf, RoomController playerRoom)
    {
        CurrentChaseTarget = ChaseTarget.FromPlayer(playerTf, playerRoom);
    }

    public void SetChaseTargetRoom(RoomController room)
    {
        CurrentChaseTarget = ChaseTarget.FromRoom(room);
    }

    public void ClearChaseTarget()
    {
        CurrentChaseTarget = default;
    }
    public void SetStateType(EnemyStateType type) => state = type;

    public void SetMove(bool move)
    {
        if (Mover != null) Mover.SetMoveEnabled(move);
    }

    public void SetMoveSpeed(float speed)
    {
        if (Mover != null) Mover.MoveSpeed = speed;
    }

    public void SetMoveMode(EnemyMoveMode mode)
    {
        MoveMode = mode;

        float targetSpeed = mode == EnemyMoveMode.Run ? runMoveSpeed : walkMoveSpeed;
        if (Mover != null) Mover.MoveSpeed = targetSpeed;
    }

    public void SetCurrentRoom(RoomController room)
    {
        CurrentRoom = room;
    }

    public void StartDoorPause(float duration)
    {
        if (duration <= 0f) return;

        if (doorPauseCoroutine != null)
        {
            StopCoroutine(doorPauseCoroutine);
        }

        doorPauseCoroutine = StartCoroutine(DoorPauseRoutine(duration));
    }

    private System.Collections.IEnumerator DoorPauseRoutine(float duration)
    {
        IsDoorPauseActive = true;
        Mover?.Stop();

        yield return new WaitForSeconds(duration);

        IsDoorPauseActive = false;
        doorPauseCoroutine = null;
    }

    public void SetDirection(float dir)
    {
        dir = Mathf.Sign(dir);
        if (dir == 0f) return;

        if (Direction != dir)
        {
            Direction = dir;
            Flip();
        }
    }

    private void Flip()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.flipX = Direction < 0f;
    }

    public void SpawnHairEffect()
    {
        if (hairPrefab != null)
        {
            Instantiate(hairPrefab, transform.position, Quaternion.identity);
        }
    }
    public void ChangeChaseState()
    {
        StateMachine.ChangeState(StateMachine.Chase);
    }
    public void ChangeDieState()
    {
        StateMachine.ChangeState(StateMachine.Die);
    }

    public void EnemyDieSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyDieCue();
    }

    public void EnemySpikeSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySpikeCue();
    }
}

public enum EnemyMoveMode
{
    Walk,
    Run,
}

public enum ChaseTargetType
{
    None,
    Player,
    Room,
}

public struct ChaseTarget
{
    public ChaseTargetType Type;
    public Transform Transform;      // Player�� player.transform, Room�̸� room.transform(�Ǵ� center anchor)
    public RoomController Room;      // Player�� player.CurrentRoom, Room�̸� �ڱ� �ڽ�

    public bool IsValid => Type != ChaseTargetType.None && Transform != null && Room != null;

    public static ChaseTarget FromPlayer(Transform playerTf, RoomController playerRoom)
        => new ChaseTarget { Type = ChaseTargetType.Player, Transform = playerTf, Room = playerRoom };

    public static ChaseTarget FromRoom(RoomController room)
        => new ChaseTarget { Type = ChaseTargetType.Room, Transform = room.transform, Room = room };

}