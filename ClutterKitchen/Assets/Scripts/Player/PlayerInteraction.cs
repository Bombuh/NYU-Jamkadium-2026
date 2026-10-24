
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 1.75f;
    [SerializeField] private LayerMask interactableLayer;

    private Camera mainCamera;
    private SpriteRenderer playerSprite;

    private void Awake()
    {
        mainCamera = Camera.main;
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private Vector2 GetPlayerCenter()
    {
        if (playerSprite != null)
        {
            return playerSprite.bounds.center;
        }

        return transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (mainCamera == null)
            return;

        // Get mouse position in world coordinates
        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePosition = new Vector2(
            mouseWorldPosition.x,
            mouseWorldPosition.y
        );

        // Find interactable object under mouse cursor
        Collider2D target = Physics2D.OverlapPoint(
            mousePosition,
            interactableLayer
        );

        if (target == null)
            return;

        IInteractable interactable =
            target.GetComponentInParent<IInteractable>();

        if (interactable == null)
            return;

        // Find the actual center of the player's sprite
        Vector2 playerCenter = GetPlayerCenter();

        // Calculate distance to the target collider
        Vector2 closestPoint = target.ClosestPoint(playerCenter);

        float distance = Vector2.Distance(
            playerCenter,
            closestPoint
        );

        // Prevent interactions outside the player's radius
        if (distance > interactionRange)
        {
            Debug.Log("Too far away to interact!");
            return;
        }

        // Interact with the clicked object
        interactable.Interact(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        SpriteRenderer sprite =
            GetComponentInChildren<SpriteRenderer>();

        Vector3 center = transform.position;

        if (sprite != null)
        {
            center = sprite.bounds.center;
        }

        Gizmos.DrawWireSphere(center, interactionRange);
    }
}


