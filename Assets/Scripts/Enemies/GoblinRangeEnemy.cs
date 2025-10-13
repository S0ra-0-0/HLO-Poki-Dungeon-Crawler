using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using HLO.Item;
public class GoblinRangeEnemy : MonoBehaviour
{
    public Player Player;
    public Inventory playerInventory;
    [Header("Stats")]
    public int Damage = 10;
    public int maxHealth = 2;
    public int currentHealth;
    public float AttackRange = 1.5f;
    private bool inRange = false;
    public float attackTimer = 0f;
    public float attackDelay = 1.5f;
    public float DetectionRange = 5.0f;
    public float AttackCooldown = 2.0f;
    public Image Hpbackground;
    public Image HpFill;
    private float lastAttackTime = -Mathf.Infinity;
    private int speed = 2;
    public GameObject projectilePrefab;
    [SerializeField] private GameObject Key;


    [Header("Hit Flash")]
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    public GameObject AttackIndicator;
    private GameObject attackIndicatorInstance;
    private Coroutine knockbackRoutine;
    [Header("Drop Item")]
    [SerializeField] private DroppedItem[] dropItems;
    [SerializeField] private float[] dropChances;

    [Header("Sprite Direction")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    public Vector2 facingDirection = Vector2.right; // Default facing right
    public Vector2 lastAttackDirection = Vector2.right; // Store the direction of the last attack

    private Coroutine attackRoutine;
    private Coroutine stunRoutine;
    [Header("State")]
    public State currentState = State.Idle;
    public enum State { Idle, Chasing, Attacking }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Player = FindAnyObjectByType<Player>();
        playerInventory = FindAnyObjectByType<Inventory>();
        currentHealth = maxHealth;
        originalMaterial = spriteRenderer.material; // Store original material
    }

    private void FixedUpdate()
    {
        if (Player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (distanceToPlayer < AttackRange)
        {
            currentState = State.Attacking;
        }
        else if (distanceToPlayer < DetectionRange)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }
        HandleState();
    }

    private void Update()
    {
        // Update direction when not attacking for idle blend tree
        if (!animator.GetBool("IsAttacking"))
        {
            UpdateSpriteDirection();
        }

        if (attackTimer >= attackDelay - 0.5f)
        {
            if (attackIndicatorInstance == null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
                attackIndicatorInstance = Instantiate(AttackIndicator, spawnPosition, Quaternion.identity, transform);
            }
            attackIndicatorInstance.SetActive(true);
        }
        else
        {
            if (attackIndicatorInstance != null)
            {
                attackIndicatorInstance.SetActive(false);
            }
        }
    }

    private void HandleState()
    {
        if (stunRoutine != null || knockbackRoutine != null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                // Update idle direction for blend tree
                SetIdleDirection();
                break;

            case State.Chasing:
                movement = directionToPlayer * speed;
                rb.linearVelocity = movement;
                // Update idle direction to match movement
                SetIdleDirection();
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                // Update idle direction to always face the player during attack preparation
                SetIdleDirection();
                if (attackRoutine == null && Time.time >= lastAttackTime + AttackCooldown)
                {
                    attackRoutine = StartCoroutine(Attack());
                    lastAttackTime = Time.time;
                }
                break;
        }
    }


    IEnumerator Attack()
    {
        // Set initial direction and trigger attack animation once
        Vector2 attackDirection = (Player.transform.position - transform.position).normalized;
        SetAttackDirection(attackDirection);
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack");
        attackTimer = 0f;

        while (Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            // Don't continuously update direction during attack - only when shooting
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                ShootProjectile();

                attackTimer = 0f;
            }

            yield return null;
        }

        // Check if attack direction is different from current facing direction after attack
        if (Vector2.Distance(lastAttackDirection, facingDirection) > 0.1f)
        {
            // Update facing direction to match attack direction
            facingDirection = lastAttackDirection;
            SetIdleDirection();
        }

        animator.SetBool("IsAttacking", false);
        attackTimer = 0f;
        attackRoutine = null;
    }
    private void ShootProjectile()
    {
        Debug.Log("Goblin shoots a projectile towards the player!");
        if (projectilePrefab != null)
        {
            Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;

            // Update attack direction for blend tree
            SetAttackDirection(directionToPlayer);

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Set the projectile's direction
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.Initialize(directionToPlayer, Damage, gameObject);
            }

            // Store the attack direction for comparison after attack finishes
            lastAttackDirection = directionToPlayer;
        }
        else
        {
            Debug.LogError("Projectile prefab not found!");
        }
    }

    private void DealDamage()
    {
        if (Player == null) return;

        Player.RegisterAttack();

        if (!Player.TryTakeDamage(Damage))
        {
            Debug.Log("Goblin's attack was blocked by parry or invuln.");
            return;
        }
    }
    private void SetIdleDirection()
    {
        animator.SetFloat("LastMoveX", facingDirection.x);
        animator.SetFloat("LastMoveY", facingDirection.y);

        UpdateSpriteDirection();
    }

    private void SetAttackDirection(Vector2 direction)
    {
        animator.SetFloat("FacingX", direction.x);
        animator.SetFloat("FacingY", direction.y);
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

    private void SetAttackDirection()
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

        animator.SetInteger("Direction", dirIndex);
    }

    public void Stun(float duration)
    {
        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(StunCoroutine(duration));
    }


    private IEnumerator StunCoroutine(float duration)
    {
        currentState = State.Idle;
        //animator.SetBool("isStunned", true);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        //animator.SetBool("isStunned", false);
        stunRoutine = null;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        HitFlash();
        HpFill.fillAmount = (float)currentHealth / maxHealth;
        if (currentHealth <= 0) Die();
        else Stun(0.5f);
    }

    public void HitFlash(float duration = 0.1f)
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

    private void Die()
    {
        for (int i = 0; i < dropItems.Length; i++)
        {
            if (dropChances[i] >= Random.value)
            {
                DropItem(dropItems[i]);
            }
        }

        Destroy(gameObject);
    }

    private void DropItem(DroppedItem item)
    {
        Instantiate(item.gameObject, (Vector2)transform.position + facingDirection.normalized * Random.value, Quaternion.identity);
    }

    public void Knockback(Vector2 knockVec)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        Stun(0.15f);

        float distance = knockVec.magnitude;
        Vector2 dir = knockVec.sqrMagnitude > 0.0001f ? knockVec.normalized : Vector2.right;

        knockbackRoutine = StartCoroutine(KnockbackRoutine(dir, distance, 0.12f));
    }

    private IEnumerator KnockbackRoutine(Vector2 dir, float distance, float duration)
    {
        if (rb == null) yield break;

        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;

        Vector2 start = rb.position;
        Vector2 end = start + dir * distance;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            Vector2 p = Vector2.Lerp(start, end, alpha);

            rb.MovePosition(p);

            yield return null;
        }

        knockbackRoutine = null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}
