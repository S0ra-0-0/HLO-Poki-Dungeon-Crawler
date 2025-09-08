using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime = -100f;
    private bool isDashing = false;
    private Vector2 dashDirection;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime = -100f;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 smoothVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
    }

    private void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        HandleDashInput();
        HandleAttackInput();
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            lastDashTime = Time.time;
            dashDirection = moveInput.magnitude > 0.1f ? moveInput.normalized : Vector2.right;
            Invoke(nameof(StopDashing), dashDuration);
        }
    }

    private void StopDashing()
    {
        isDashing = false;
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            Debug.Log("Attack!");
            Invoke(nameof(StopAttacking), 0.1f);
        }
    }

    private void StopAttacking()
    {
        isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            Vector2 targetVelocity = moveInput * moveSpeed;
            rb.linearVelocity = Vector2.SmoothDamp(
                rb.linearVelocity,
                targetVelocity,
                ref smoothVelocity,
                acceleration > 0 ? 1f / acceleration : 0.1f
            );

            // Apply deceleration when there's no input
            if (moveInput.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw dash indicator when dashing
        if (isDashing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.8f);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dashDirection * 1.5f);
        }

        // Draw attack range when attacking
        if (isAttacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 attackDirection = (mousePosition - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + attackDirection * attackRange);
        }
    }
}
