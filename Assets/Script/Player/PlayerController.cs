using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Movement parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f; // Giảm lực nhảy để tránh bay quá cao
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f; // Tăng bán kính để phát hiện tốt hơn
    [SerializeField] private float maxVelocityY = 10f; // Giới hạn tốc độ tối đa theo trục Y

    // State variables
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;
    private bool isDead = false;

    // Animation parameters
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int DieType = Animator.StringToHash("DieType");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Có thể null nếu không gắn

        // Lock Z position and rotations
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                        RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY |
                        RigidbodyConstraints.FreezeRotationZ;

        // Đảm bảo gravity hoạt động
        rb.useGravity = true;
    }

    void Update()
    {
        if (isDead) return;

        // Check if grounded with debug log
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log("IsGrounded: " + isGrounded); // Bật log để kiểm tra

        // Get input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f); // Reset velocity Y trước khi nhảy
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool(IsJumping, true);
        }

        // Update animations
        animator.SetBool(IsRunning, Mathf.Abs(moveInput) > 0);
        animator.SetBool(IsJumping, !isGrounded);

        // Flip sprite if available
        if (spriteRenderer != null && moveInput != 0)
        {
            if (moveInput > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput < 0 && isFacingRight)
            {
                Flip();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Move player with velocity limit
        Vector3 velocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = new Vector3(velocity.x, Mathf.Clamp(rb.linearVelocity.y, -maxVelocityY, maxVelocityY), 0f);

        // Ensure Z position is locked
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void Flip()
    {
        if (spriteRenderer != null)
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    // Call this to trigger death with specific die type (0, 1, 2, etc.)
    public void TriggerDeath(int dieType)
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetInteger(DieType, dieType);
        animator.SetTrigger(Die);
    }

    // Called by Animation Event at the end of die animation
    public void OnDeathAnimationComplete()
    {
        // Handle post-death logic (e.g., respawn, game over)
        Debug.Log("Death animation completed");
        // Example: Destroy(gameObject);
    }
}

/*
 Để kích hoạt chết với kiểu cụ thể, gọi hàm TriggerDeath từ script khác hoặc trong điều kiện game. Ví dụ:
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        GetComponent<PlayerController>().TriggerDeath(0); // Chết do va chạm
    }
}

void Update()
{
    if (transform.position.y < -10) // Rơi khỏi map
    {
        GetComponent<PlayerController>().TriggerDeath(1); // Chết do rơi
    }
}
 */
