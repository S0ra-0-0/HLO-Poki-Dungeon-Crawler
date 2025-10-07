using System.Collections;
using HLO.Item;
using UnityEngine;

public class OgerMiniBoss : MonoBehaviour
{
    public Player Player;
    [Header("Stats")]
    public int Damage = 1;
    public int maxHealth = 10;
    public int currentHealth;
    public float AttackRange = 2.0f;
    private bool inRange = false;
    public float attackTimer = 0f;
    public float attackDelay = 2.0f;
    public float DetectionRange = 7.0f;
    public float AttackCooldown = 3.0f;
    private float lastAttackTime = -Mathf.Infinity;
    public int speed = 1;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Coroutine attackRoutine;
    private Vector2 movement;

    [Header("Attack Indicator")]
    public GameObject AttackIndicator;
    [SerializeField] float attackIndicatorOffset = 1.35f;
    private GameObject attackIndicatorInstance;

    [SerializeField] private float attackEffectPosOffset = 1f;
    [SerializeField] private float attackEffectRotOffset = -90f;

    private Coroutine stunRoutine;
    private Coroutine knockbackRoutine;


    [Header("Hit Flash")]
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    [Header("Drop Item")]
    [SerializeField] private DroppedItem[] dropItems;
    [SerializeField] private float[] dropChances;

    [Header("Sprites")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    public Vector2 facingDirection = Vector2.right; // Default facing right
    public GameObject clubWeapon;

    public State currentState = State.Idle;
    public enum State { Idle, Chasing, Attacking }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Player = FindAnyObjectByType<Player>();
        currentHealth = maxHealth;
        originalMaterial = spriteRenderer.material;
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
        UpdateSpriteDirection();

        if (attackTimer >= attackDelay - 0.5f)
        {
            if (attackIndicatorInstance == null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, attackIndicatorOffset, 0);
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
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Chasing:
                movement = directionToPlayer * speed;
                rb.linearVelocity = movement;
                facingDirection = directionToPlayer;
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                facingDirection = directionToPlayer;
                if (attackRoutine == null && Time.time >= lastAttackTime + AttackCooldown)
                {
                    attackRoutine = StartCoroutine(Attack());
                    lastAttackTime = Time.time;
                }
                break;
        }
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

    IEnumerator Attack()
    {
        attackTimer = 0f;

        while (Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                GameObject weaponInstance = Instantiate(
                    clubWeapon,
                    (Vector2)transform.position + facingDirection.normalized * attackEffectPosOffset,
                    Quaternion.Euler(0f, 0f, Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg + attackEffectRotOffset)
                );

                Destroy(weaponInstance, 0.3f);
                DealDamage();
                attackTimer = 0f;
            }

            yield return null;
        }

        attackTimer = 0f;
        attackRoutine = null;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }

    private void DealDamage()
    {
        if (Player == null) return;

        Player.RegisterAttack();

        if (!Player.TryTakeDamage(Damage))
        {
            Debug.Log("Ogre boss's attack was blocked.");
            return;
        }
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
        //HpFill.fillAmount = (float)currentHealth / maxHealth;
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
            if (dropChances[i] >= Random.Range(0f, 1f))
            {
                DropItem(dropItems[i]);
            }
        }

        // Temp Code
        // TODO: Fix that Pleeeeeeeeeeeeeeeeeeeeeeeease.
        UnityEngine.SceneManagement.SceneManager.LoadScene("win screen");

        // Destroy(gameObject);
    }

    private void DropItem(DroppedItem item)
    {
        Instantiate(item.gameObject, (Vector2)transform.position + facingDirection.normalized * Random.Range(0f, 1f), Quaternion.identity);
    }
}
