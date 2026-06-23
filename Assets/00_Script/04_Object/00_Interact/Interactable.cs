using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("성공/실패 메시지")]
    [SerializeField] protected string successMessage;
    [SerializeField] protected string failMessage;

    [Header("우선순위/기본 우선순위 설정")]
    [SerializeField] protected int priority;
    private int originalPriority;

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;
    public int Priority => priority;

    protected Player player;
    protected bool inRange;

    protected virtual void Awake()
    {
        originalPriority = priority;
        if (outlineMaterial == null)
        {
            outlineMaterial = Resources.Load<Material>("Outline");
        }
        if (defaultMaterial == null)
        {
            defaultMaterial = Resources.Load<Material>("Sprite-Lit-Default");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Player>(out var triggerPlayer))
        {
            player = triggerPlayer;
            inRange = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<Player>(out var triggerPlayer) && player != null)
        {
            player = null;
            inRange = false;
        }
    }

    public abstract void Interact();
    protected void ShowSuccess() => UIManager.Instance.ShowMessage(successMessage);

    protected void ShowFail() => UIManager.Instance.ShowMessage(failMessage);

    public void SetPriority(int value) => priority = value;

    public void ResetPriority() => priority = originalPriority;

    public void SetMaterial(Material material)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.material = material;
        }
    }

    public virtual void SetInteractable()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SetMaterial(outlineMaterial);
        }
    }

    public virtual void SetDefault()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SetMaterial(defaultMaterial);
        }
    }
}
