
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwitchInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activeSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActive = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (defaultSprite == null)
            defaultSprite = spriteRenderer.sprite;
    }

    public void Interact(PlayerInteraction player)
    {
        isActive = !isActive;

        spriteRenderer.sprite = isActive
            ? activeSprite
            : defaultSprite;
    }
}
