using UnityEngine;
using System.Collections;

public class OgerMiniBoss : MonoBehaviour
{
 public Player Player;
    public int Damage = 20;
    public int maxHealth = 10;
    public int currentHealth;
    public float AttackRange = 2.0f;
    private bool inRange = false;   
    public float attackTimer = 0f;
    public float attackDelay = 2.0f;
    public float DetectionRange = 7.0f;
    public float AttackCooldown = 3.0f;
    private float lastAttackTime = -Mathf.Infinity;
    private int speed = 1;
    private Rigidbody2D rb;

    [Header("Sprite Direction")]
    [SerializeField] private Sprite[] idleDirectionSprites = new Sprite[8]; // Assign in Inspector: Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    public Vector2 facingDirection = Vector2.right; // Default facing right

    public State currentState = State.Idle;
    public enum State { Idle, Chasing, Attacking }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }
}
