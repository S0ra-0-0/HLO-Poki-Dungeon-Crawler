using UnityEngine;
using System.Collections;

public class SwordAttack : IAttackType
{
    public float Cooldown => 0.5f;
    private const float parryWindow = 0.3f;
    private const float parryCooldown = 1.0f;

    public void Attack(Player player)
    {
        Debug.Log("Sword Attack!");
        // Use OverlapCircleAll to approximate an arc by filtering by angle
        var enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, LayerMask.GetMask("Enemy"));
        float minAngle = Vector2.SignedAngle(Vector2.right, player.facingDirection) - 45f;
        float maxAngle = Vector2.SignedAngle(Vector2.right, player.facingDirection) + 45f;
        foreach (var enemy in enemies)
        {
            Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)player.transform.position;
            float angle = Vector2.SignedAngle(player.facingDirection, toEnemy.normalized);
            if (angle >= -60f && angle <= 60f)
            {
                enemy.SendMessage("TakeDamage", 3, SendMessageOptions.DontRequireReceiver);
            }
        }

#if UNITY_EDITOR
        // Draw the arc/slash for the attack
        DrawAttackArc(player.transform.position, player.facingDirection, 1.5f, 90f, Color.magenta, 0.2f);
#endif
    }

    public void heavyAttack(Player player)
    {
        player.StartCoroutine(ParryCoroutine(player));
    }

    private IEnumerator ParryCoroutine(Player player)
    {
        player.isHeavyAttacking = true;
        player.parrySuccess = false;
        
        // Store original state
        var rb = player.GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Create gizmo drawer for parry box
        var drawer = player.gameObject.AddComponent<ParryGizmoDrawer>();
        drawer.Init(1.0f, 0.8f, 0.5f);

        float timer = 0f;
        
        // Parry window
        while (timer < parryWindow && !player.parrySuccess)
        {
            timer += Time.deltaTime;
            
            if (player.WasAttackedThisFrame())
            {
                player.parrySuccess = true;
                Debug.Log("Parry Success! Counterattack enabled.");
                player.TriggerCounterAttack();
                break;
            }
            
            yield return null;
        }

        // Cleanup
        Object.Destroy(drawer);
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = originalVelocity;
        }
        player.isHeavyAttacking = false;
        
        if (!player.parrySuccess)
        {
            Debug.Log("Parry missed.");
        }
    }

#if UNITY_EDITOR
    // Helper to draw an arc in the Scene view
    private void DrawAttackArc(Vector2 center, Vector2 facing, float radius, float angle, Color color, float duration)
    {
        int segments = 16;
        float halfAngle = angle / 2f;
        float startAngle = Vector2.SignedAngle(Vector2.right, facing) - halfAngle;
        float deltaAngle = angle / segments;

        Vector2 prevPoint = center + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right * radius);
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + deltaAngle * i;
            Vector2 nextPoint = center + (Vector2)(Quaternion.Euler(0, 0, currentAngle) * Vector2.right * radius);
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
        }
        // Optionally, draw lines from center to arc ends
        Vector2 arcStart = center + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right * radius);
        Vector2 arcEnd = center + (Vector2)(Quaternion.Euler(0, 0, startAngle + angle) * Vector2.right * radius);
        Debug.DrawLine(center, arcStart, color, duration);
        Debug.DrawLine(center, arcEnd, color, duration);
    }

    // Gizmo drawer for the parry box
    public class ParryGizmoDrawer : MonoBehaviour
    {
        private float distance;
        private float width;
        private float height;
        private bool isActive = true;

        public void Init(float distance, float width, float height)
        {
            this.distance = distance;
            this.width = width;
            this.height = height;
        }

        private void OnDrawGizmos()
        {
            if (!isActive) return;
            
            var player = GetComponent<Player>();
            if (player == null) return;

            Vector2 facing = player.facingDirection.normalized;
            Vector2 right = new Vector2(-facing.y, facing.x);
            
            Vector2 boxCenter = (Vector2)transform.position + facing * distance;
            Vector2 halfRight = right * (width / 2f);
            Vector2 halfForward = facing * (height / 2f);

            Vector2 topLeft = boxCenter - halfRight + halfForward;
            Vector2 topRight = boxCenter + halfRight + halfForward;
            Vector2 bottomLeft = boxCenter - halfRight - halfForward;
            Vector2 bottomRight = boxCenter + halfRight - halfForward;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
#endif
}