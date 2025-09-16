using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float heavyAttackCooldown = 1.0f;
    private float lastNormalAttackTime = -100f;
    public float lastHeavyAttackTime = -100f;
    public bool isAttacking = false;
    public bool isHeavyAttacking = false;
    public bool isChargingHeavy = false;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Sprite Direction")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight

    [Header("Hit Flash")]
    [SerializeField] private Material flashMaterial; // Assign FlashMaterial in Inspector
    private Material originalMaterial;
    private Coroutine flashRoutine;

    [Header("Weapons")]
    public GameObject swordPrefab;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip swordSwingSound;
    public AudioClip parrySound;
    public AudioClip dashSound;
    public AudioClip hitSound;
    public AudioClip walkingSound;
    public float walingClipLength = 1f;
    float timer;
    public float volume = 1;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 smoothVelocity;
    private SpriteRenderer spriteRenderer;

    private List<IAttackType> attackTypes;
    private int currentAttackIndex = 0;
    private IAttackType currentAttackType;

    public Vector2 facingDirection = Vector2.right; // Default facing right

    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable || isDashing;

    private bool actionsDisabled = false;
    private bool attackedThisFrame = false;
    public bool parrySuccess = false;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material; // Store original material

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

    public void FlashRed(float duration = 0.1f)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        // Switch to flash material
        spriteRenderer.material = flashMaterial;
        flashMaterial.SetFloat("_FlashAmount", 1f); // Full flash

        yield return new WaitForSeconds(duration);

        // Revert to original material
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > walingClipLength)
        {
            audioSource.PlayOneShot(walkingSound);
            timer = 0;
        }


        if (!isHeavyAttacking || !actionsDisabled)
        {
            HandleMovement();
        }
        else
        {
            HandleMovement();

            // Prevent movement while heavy attacking
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        UpdateSpriteDirection();
        HandleDashInput();
        HandleAttackInput();
        HandleAttackSwapInput();
    }

    private void HandleMovement()
    {
        moveInput = new Vector2(
           Input.GetAxisRaw("Horizontal"),
           Input.GetAxisRaw("Vertical")
       ).normalized;

        // Update facing direction if moving
        if (moveInput.sqrMagnitude > 0.01f)
            facingDirection = Get8Direction(moveInput);
    }
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            lastDashTime = Time.time;
            dashDirection = moveInput.magnitude > 0.1f ? moveInput.normalized : Vector2.right;
            audioSource.PlayOneShot(dashSound);
            Invoke(nameof(StopDashing), dashDuration);
        }
    }

    private void StopDashing()
    {
        isDashing = false;
    }
    private void HandleAttackInput()
    {
        // Heavy attack (parry)
        if (Input.GetKeyDown(KeyCode.K)
            && !isAttacking && !isHeavyAttacking
            && Time.time >= lastHeavyAttackTime + heavyAttackCooldown)
        {
            currentAttackType.heavyAttack(this);
        }

        // Normal attack (unchanged)
        if (Input.GetKeyUp(KeyCode.J) && !isAttacking && !isHeavyAttacking
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.8f);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dashDirection * 1.5f);
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

    private void UpdateSpriteDirection()
    {
        // 0: Right, 1: UpRight, 2: Up, 3: UpLeft, 4: Left, 5: DownLeft, 6: Down, 7: DownRight
        int dirIndex = 0;
        Vector2 dir = facingDirection.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        if (angle >= 337.5f || angle < 22.5f) dirIndex = 0; // Right
        else if (angle >= 22.5f && angle < 67.5f) dirIndex = 1; // UpRight
        else if (angle >= 67.5f && angle < 112.5f) dirIndex = 2; // Up
        else if (angle >= 112.5f && angle < 157.5f) dirIndex = 3; // UpLeft
        else if (angle >= 157.5f && angle < 202.5f) dirIndex = 4; // Left
        else if (angle >= 202.5f && angle < 247.5f) dirIndex = 5; // DownLeft
        else if (angle >= 247.5f && angle < 292.5f) dirIndex = 6; // Down
        else if (angle >= 292.5f && angle < 337.5f) dirIndex = 7; // DownRight

        if (idleDirectionSprites != null && idleDirectionSprites.Length == 8)
            spriteRenderer.sprite = idleDirectionSprites[dirIndex];
    }

    public void SetInvulnerable(bool value)
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        playerHealth.isInvincibility = value;

    }

    public void DisableActions(bool value)
    {
        actionsDisabled = value;
    }

    public bool WasAttackedThisFrame()
    {
        bool result = attackedThisFrame;
        attackedThisFrame = false;
        return result;
    }


    public bool TryTakeDamage(int damage)
    {
        RegisterAttack();

        if (parrySuccess)
        {
            Debug.Log("Parry successful! No damage taken.");
            parrySuccess = false;
            TriggerCounterAttack();
            return false;
        }

        if (isInvulnerable || isDashing)
        {
            Debug.Log("Player invulnerable, no damage taken.");
            return false;
        }

        Debug.Log($"Player takes {damage} damage.");
        FlashRed(); // Flash red when hit
        audioSource.PlayOneShot(hitSound, volume);
        GetComponent<PlayerHealth>().Hit(damage);
        return true;
    }


    public void RegisterAttack()
    {
        attackedThisFrame = true;
        Debug.Log("[Player] Attack registered this frame!");
    }


    public void TriggerCounterAttack()
    {
        Debug.Log("Counterattack triggered!");

        var enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Enemy"));
        foreach (var enemy in enemies)
        {
            Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)transform.position;
            float angle = Vector2.SignedAngle(facingDirection, toEnemy.normalized);

            if (angle >= -60f && angle <= 60f)
            {
                enemy.SendMessage("TakeDamage", 5, SendMessageOptions.DontRequireReceiver);
                Debug.Log($"Counter hit {enemy.name}");
            }
        }

        StartCoroutine(CounterFreezeFrames(.5f));
    }

    private IEnumerator CounterFreezeFrames(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    public void SetParryVisual(bool isActive)
    {
        if (isActive)
        {
            spriteRenderer.color = Color.blue; // Glow effect during parry
        }
        else
        {
            spriteRenderer.color = Color.white; // Restore normal color
        }
    }


}
