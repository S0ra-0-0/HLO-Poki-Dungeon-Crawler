using System.Collections;
using UnityEngine;
public class GoblinEnemy : MonoBehaviour
{
    public Player Player;
    public int Damage = 10;
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

    private Coroutine attackRoutine;

    public State currentState = State.Idle;
    public enum State { Idle, Chasing, Attacking }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
    private void HandleState()
    {
        if (stunRoutine != null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                //animator.SetBool("isRunning", false);
                rb.linearVelocity = Vector2.zero;
                break;
            case State.Chasing:
                //animator.SetBool("isRunning", true);
                Vector2 direction = (Player.transform.position - transform.position).normalized;
                movement = direction * speed;
                rb.linearVelocity = movement;
                break;
            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                //animator.SetBool("isRunning", false);
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

    private Coroutine stunRoutine;

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
       
        if (attackTimer >= attackDelay - 0.2f) { Gizmos.color = Color.magenta; }
        else { Gizmos.color = Color.blue; }
        Gizmos.DrawWireSphere(transform.position, AttackRange);
      

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}
