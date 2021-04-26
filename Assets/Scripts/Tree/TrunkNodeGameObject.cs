using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class TrunkNodeGameObject : MonoBehaviour, ISelectable
{
    [Header("References:")]
    [SerializeField] private BuildMenuUI buildMenuUI;
    
    public TrunkNode Data { get; set; }
    public UnityAction OnSelect { get; set; }
    public UnityAction OnDeselect { get; set; }

    private SpriteRenderer spriteRenderer;
    private Color orgColor;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        orgColor = spriteRenderer.color;

        OnDeselect += () => {
            if(spriteRenderer != null)
                spriteRenderer.color = orgColor;
        };
    }

    private void Update()
    {
        if (Data != null)
            transform.position = Data.WorldPosition;   
    }

    public void OnMouseDown()
    {
        UIManager.Instance.Selected = this;
    }

    public void OnMouseOver()
    {
        spriteRenderer.color = Color.green;
    }

    public void OnMouseExit()
    {
        if ((object)UIManager.Instance.Selected != this)
        {
            spriteRenderer.color = orgColor;
        }
    }
}
