using UnityEngine;

public class Bullet : MonoBehaviour
{
   [SerializeField] private float projectileSpeed = 10f;
   [SerializeField] private float lifetime = 5f;
   [SerializeField] private int damage = 1;
   private float lifeTimer;
   private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Ensure no gravity in 2D
    }

    private void OnEnable()
    {
        lifeTimer = lifetime;
    }
    private void Update()
    {
        // Decrease lifetime
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ignore collisions with the player
            return;
        }

        collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);    
    }

}
