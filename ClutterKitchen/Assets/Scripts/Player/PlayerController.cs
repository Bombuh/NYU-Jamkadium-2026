
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 movement;

    // Facing directions:
    // 0 = Down
    // 1 = Up
    // 2 = Side
    private int facing = 0;

    // Public properties for other scripts
    public Vector2 Movement => movement;
    public bool IsMoving => movement != Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Automatically find animation components
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Prevent physics from rotating the character
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimations();
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Read WASD input.
        // Horizontal movement takes priority.
        if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1f;
        else if (Input.GetKey(KeyCode.W))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S))
            vertical = -1f;

        movement = new Vector2(horizontal, vertical);
    }

    private void HandleAnimations()
    {
        bool isMoving = IsMoving;

        if (isMoving)
        {
            // Moving left or right
            if (movement.x != 0)
            {
                facing = 2;

                if (spriteRenderer != null)
                {
                    // Original sprite faces right
                    spriteRenderer.flipX = movement.x < 0;
                }
            }
            // Moving upward
            else if (movement.y > 0)
            {
                facing = 1;
                ResetSpriteFlip();
            }
            // Moving downward
            else if (movement.y < 0)
            {
                facing = 0;
                ResetSpriteFlip();
            }
        }

        // Update Animator parameters
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetInteger("Facing", facing);
        }
    }

    private void ResetSpriteFlip()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }
}
