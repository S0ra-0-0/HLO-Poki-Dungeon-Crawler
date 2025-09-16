using System.Collections;
using System.Net.Sockets;
using Mono.Cecil;
using UnityEngine;
public class GoblinEnemy : MonoBehaviour
{
    public Player Player;
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
    private float lastAttackTime = -Mathf.Infinity;
    private int speed = 2;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    public GameObject AttackIndicator;
    private GameObject attackIndicatorInstance;



    [Header("Sprite Direction")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    public Vector2 facingDirection = Vector2.right; // Default facing right

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
        currentHealth = maxHealth;
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

        if (attackTimer >= attackDelay - 0.6f)
        {
            if (attackIndicatorInstance == null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, .5f, 0); 
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
        if (stunRoutine != null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Chasing:
                movement = directionToPlayer * speed;
                rb.linearVelocity = movement;
                // Update facing direction to match movement
                facingDirection = directionToPlayer;
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                // Update facing direction to always face the player during attack
                facingDirection = directionToPlayer;
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
        //animator.SetTrigger("attack");
        attackTimer = 0f;

        while (Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                DealDamage();
                attackTimer = 0f;
            }

            yield return null;
        }

        attackTimer = 0f;
        attackRoutine = null;
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


    private void DealDamage()
    {
        if (Player == null) return;

        // Explicitly tell the player it was attacked
        Player.RegisterAttack();

        if (!Player.TryTakeDamage(Damage))
        {
            Debug.Log("Goblin's attack was blocked by parry or invuln.");
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

    private void OnDrawGizmos()
    {

        if (attackTimer >= attackDelay - 0.6f) { Gizmos.color = Color.magenta; }
        else { Gizmos.color = Color.blue; }
        Gizmos.DrawWireSphere(transform.position, AttackRange);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0) Die();
        else Stun(0.5f);
    }


    private void Die()
    {
        // Play death animation or effects here maybe
        Destroy(gameObject);
    }
}
