using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float normalAttackCooldown = 0.3f;
    [SerializeField] private float heavyAttackCooldown = 1.0f;
    private float lastNormalAttackTime = -100f;
    private float lastHeavyAttackTime = -100f;
    private bool isAttacking = false;
    public   bool isHeavyAttacking = false;
    private bool isChargingHeavy = false;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;


    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 smoothVelocity;

    private List<IAttackType> attackTypes;
    private int currentAttackIndex = 0;
    private IAttackType currentAttackType;

    public Vector2 facingDirection = Vector2.right; // Default facing right

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Initialize attack types
        attackTypes = new List<IAttackType>
        {
            new SwordAttack(),
            new SpearAttack(),
            new RangedAttack(),
            new SummonerAttack()
        };
        currentAttackType = attackTypes[currentAttackIndex];
    }

    private void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Update facing direction if moving
        if (moveInput.sqrMagnitude > 0.01f)
            facingDirection = Get8Direction(moveInput);

        HandleDashInput();
        HandleAttackInput();
        HandleAttackSwapInput();
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
        // Start heavy attack charge on F down
        if (Input.GetKeyDown(KeyCode.F)
            && !isAttacking && !isHeavyAttacking
            && Time.time >= lastHeavyAttackTime + heavyAttackCooldown)
        {
            isHeavyAttacking = true;
            isChargingHeavy = true;
            lastHeavyAttackTime = Time.time;
            currentAttackType.heavyAttack(this); // Starts coroutine, handles charge
            return;
        }
        // Release heavy attack on F up
        if (isChargingHeavy && isHeavyAttacking && Input.GetKeyUp(KeyCode.F))
        {
            isChargingHeavy = false;
            HeavySpearGizmoDrawer.ReleaseCharge(); // Static call to notify coroutine to release
            return;
        }
        // Normal attack
        if (Input.GetMouseButtonDown(0)
            && !isAttacking && !isHeavyAttacking
            && Time.time >= lastNormalAttackTime + normalAttackCooldown)
        {
            isAttacking = true;
            lastNormalAttackTime = Time.time;
            currentAttackType.Attack(this);
            Invoke(nameof(StopAttacking), 0.1f);
        }
    }

    private void HandleAttackSwapInput()
    {
        //Temp Press Q/E to cycle attack types
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentAttackIndex = (currentAttackIndex - 1 + attackTypes.Count) % attackTypes.Count;
            currentAttackType = attackTypes[currentAttackIndex];
            Debug.Log($"Switched to {currentAttackType.GetType().Name}");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentAttackIndex = (currentAttackIndex + 1) % attackTypes.Count;
            currentAttackType = attackTypes[currentAttackIndex];
            Debug.Log($"Switched to {currentAttackType.GetType().Name}");
        }
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

    // Helper to snap to 8 directions
    private Vector2 Get8Direction(Vector2 input)
    {
        if (input == Vector2.zero) return facingDirection;
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        if (angle >= 337.5f || angle < 22.5f) return Vector2.right;
        if (angle >= 22.5f && angle < 67.5f) return (Vector2.right + Vector2.up).normalized;
        if (angle >= 67.5f && angle < 112.5f) return Vector2.up;
        if (angle >= 112.5f && angle < 157.5f) return (Vector2.left + Vector2.up).normalized;
        if (angle >= 157.5f && angle < 202.5f) return Vector2.left;
        if (angle >= 202.5f && angle < 247.5f) return (Vector2.left + Vector2.down).normalized;
        if (angle >= 247.5f && angle < 292.5f) return Vector2.down;
        if (angle >= 292.5f && angle < 337.5f) return (Vector2.right + Vector2.down).normalized;
        return facingDirection;
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

        // Draw facing direction indicator
        Gizmos.color = Color.green;
        Vector3 headPos = transform.position + Vector3.up * 0.7f;
        Gizmos.DrawSphere(headPos, 0.08f);
        Gizmos.DrawLine(headPos, headPos + (Vector3)facingDirection * 0.4f);
    }

    private void StopAttacking()
    {
        isAttacking = false;
    }

    private void StopHeavyAttacking()
    {
        isHeavyAttacking = false;
    }
}
