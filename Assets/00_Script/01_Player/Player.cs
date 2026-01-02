using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerStateType state;
    public PlayerStateType State => state;

    public PlayerInputReader PlayerInputReader { get; private set; }
    public PlayerMover PlayerMover { get; private set; }
    public PlayerInteraction PlayerInteraction { get; private set; }
    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerStamina PlayerStamina { get; private set; }
    public Rigidbody2D Rigidbody2D { get; private set; }
    public BoxCollider2D _Collider2D { get; private set; }

    public float LastHideTime { get; private set; }
    public bool IsHidden { get; private set; }

    public RoomController CurrentRoom { get; private set; }
    public PlayerInventory PlayerInventory { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        PlayerInputReader = GetComponent<PlayerInputReader>();
        PlayerMover = GetComponent<PlayerMover>();
        PlayerInteraction = GetComponentInChildren<PlayerInteraction>();
        PlayerStamina = GetComponent<PlayerStamina>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        _Collider2D = GetComponent<BoxCollider2D>();
        PlayerInventory = GetComponent<PlayerInventory>();

        PlayerStateMachine = new PlayerStateMachine(this);
    }

    private void OnEnable()
    {
        if (PlayerInputReader != null)
        {
            PlayerInputReader.Jump += OnJumpPressed;
        }

        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedPlayerRoom += SetCurrentRoom;
    }

    private void OnDisable()
    {
        if (PlayerInputReader != null)
        {
            PlayerInputReader.Jump -= OnJumpPressed;
        }

        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedPlayerRoom -= SetCurrentRoom;
    }

    private void Update()
    {
        PlayerStateMachine.Update();
    }

    private void OnJumpPressed()
    {
        PlayerStateMachine.RequestJump();
    }

    public void SetStateType(PlayerStateType type) => state = type;

    public void SetHidden(bool hidden)
    {
        IsHidden = hidden;
        gameObject.layer = hidden ? LayerMask.NameToLayer("PlayerHidden") : LayerMask.NameToLayer("Player");

        if (hidden)
            LastHideTime = Time.time;
    }

    public void SetCurrentRoom(RoomController room) => CurrentRoom = room;
}
