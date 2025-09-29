using UnityEngine;
using System.Collections;

public class OgerMiniBoss : MonoBehaviour
{
    public Player Player;
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
    public GameObject AttackIndicator;
    private GameObject attackIndicatorInstance;


    [Header("Sprite Direction")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    public Vector2 facingDirection = Vector2.right; // Default facing right

    public State currentState = State.Idle;
    public enum State { Idle, Chasing, Attacking }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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

        if (attackTimer >= attackDelay - 0.5f)
        {
            if (attackIndicatorInstance == null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, .75f, 0);
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
        //animator.SetTrigger("attack");
        attackTimer = 0f;

        while (Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                //attack();
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
}
