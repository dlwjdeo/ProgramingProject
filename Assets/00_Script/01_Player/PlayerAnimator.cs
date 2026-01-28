using UnityEngine;

public sealed class PlayerAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

    [Header("Params")]
    [SerializeField] private string locomotionParam = "Locomotion";

    private Player _player;
    private int _locomotionHash;

    private void Awake()
    {
        _player = GetComponent<Player>();

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (visualRoot == null) visualRoot = transform;

        _locomotionHash = Animator.StringToHash(locomotionParam);
    }

    private void Update()
    {
        if (_player == null || animator == null) return;

        int loco = _player.State switch
        {
            PlayerStateType.Idle => 0,
            PlayerStateType.Walk => 1,
            PlayerStateType.Run => 2,
            _ => 0,
        };

        animator.SetInteger(_locomotionHash, loco);

        float mx = _player.PlayerMover != null ? _player.PlayerMover.MoveInput.x : 0f;
        if (Mathf.Abs(mx) > 0.01f)
        {
            Vector3 s = visualRoot.localScale;
            s.x = Mathf.Abs(s.x) * (mx > 0f ? 1f : -1f);
            visualRoot.localScale = s;
        }
    }
}
