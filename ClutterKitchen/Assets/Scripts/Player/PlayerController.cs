using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Read raw input
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        else if (Input.GetKey(KeyCode.W)) vertical = 1f;
        else if (Input.GetKey(KeyCode.S)) vertical = -1f;

        movement = new Vector2(horizontal, vertical);

        // Flip sprite only when moving left/right
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}