using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;
    private Vector2 direction;
    [SerializeField] private float speed = 10f; // Adjust speed as needed
    private GameObject shooter; // Reference to the enemy that shot the projectile

    public void Initialize(Vector2 direction, int damage, GameObject shooter)
    {
        this.direction = direction;
        this.damage = damage;
        this.shooter = shooter; // Store the shooter reference
    }

    private void Update()
    {
        // Move the projectile
        transform.Translate(direction * speed * Time.deltaTime);

        Destroy(gameObject, 5f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile collided with {collision.gameObject.name} with tag{collision.gameObject.tag}");
      
        if (collision.gameObject == shooter && !gameObject.CompareTag("DeflectedProjectile"))
        {
            return; // Ignore collision with the shooter
        }

        if (collision.CompareTag("Player") && !gameObject.CompareTag("DeflectedProjectile"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.RegisterAttack();
                player.TryTakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy") && gameObject.CompareTag("DeflectedProjectile"))
        {
            collision.SendMessage("TakeDamage", 5, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Enemy") || collision.CompareTag("DeflectedProjectile"))
        {
            Destroy(gameObject);
        }
    }

    private void Deflect(Vector2 reflection)
    {
        direction = reflection.normalized;
        gameObject.tag = "DeflectedProjectile";
        gameObject.GetComponent<SpriteRenderer>().color = Color.green; 
    }
}
