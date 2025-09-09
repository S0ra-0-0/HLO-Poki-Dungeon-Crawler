using UnityEngine;
#if UNITY_EDITOR
#endif

public class SpearAttack : IAttackType
{
    public float Cooldown => 0.2f;

    private const float spearReach = 2.5f;
    private const float hitRadius = .5f;
    private const int spearDamage = 2;

    // Heavy attack config
    private readonly float[] stageTimes = { 0.25f, .5f, 1f };
    private readonly int[] stageDamages = { 4, 8, 12 };
    private readonly float[] stageRadii = { .75f, 1.5f, 3.0f };

    public void Attack(Player player)
    {
        Debug.Log("Basic Spear Attack!");

        Vector2 facing = player.facingDirection != Vector2.zero ? player.facingDirection : Vector2.right;
        Vector2 attackPosition = (Vector2)player.transform.position + facing * spearReach;

        int enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, hitRadius, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            enemy.SendMessage("TakeDamage", spearDamage, SendMessageOptions.DontRequireReceiver);
        }

#if UNITY_EDITOR
        Debug.DrawLine(player.transform.position, attackPosition, Color.magenta, 0.2f);
#endif
    }

    // Called when charge starts
    public void heavyAttack(Player player)
    {
        player.StartCoroutine(HeavySpearChargeCoroutine(player));
    }

    // Coroutine for charge attack
    private System.Collections.IEnumerator HeavySpearChargeCoroutine(Player player)
    {
        player.isHeavyAttacking = true;
        float timer = 0f;
        int stage = 0;

        var rb = player.GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        var drawer = player.gameObject.AddComponent<HeavySpearGizmoDrawer>();
        drawer.Init(stageRadii[stage], stageRadii[stageRadii.Length - 1]);

        HeavySpearGizmoDrawer.ResetRelease();

        // Wait while Left Shift is held, or until max charge, or until released
        while (timer < stageTimes[stageTimes.Length - 1] && !HeavySpearGizmoDrawer.ShouldRelease)
        {
            timer += Time.deltaTime;
            // Update stage
            for (int i = stageTimes.Length - 1; i >= 0; i--)
            {
                if (timer >= stageTimes[i])
                {
                    stage = i;
                    break;
                }
            }
            // Smoothly grow the radius
            float t = Mathf.Clamp01(timer / stageTimes[stageTimes.Length - 1]);
            drawer.SetRadius(Mathf.Lerp(stageRadii[0], stageRadii[stageRadii.Length - 1], t));
            yield return null;
        }

        // Final radius and damage based on charge
        float finalRadius = drawer.CurrentRadius;
        int finalDamage = stageDamages[stage];

        // Visual feedback for attack (optional)
        drawer.SetAttackFlash();

        // Damage all enemies in the final circle
        int enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, finalRadius, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            enemy.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);
        }

        yield return new WaitForSeconds(0.2f); // Show flash briefly

        Object.Destroy(drawer);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = originalVelocity;
        player.isHeavyAttacking = false;
    }
}

// Helper MonoBehaviour to draw the growing circle using Gizmos
public class HeavySpearGizmoDrawer : MonoBehaviour
{
    private float radius = .75f;
    private float maxRadius = 3.0f;
    private bool flash = false;
    public float CurrentRadius => radius;

    private static bool shouldRelease = false;
    public static bool ShouldRelease => shouldRelease;

    public static void ReleaseCharge() => shouldRelease = true;
    public static void ResetRelease() => shouldRelease = false;

    public void Init(float startRadius, float maxRadius)
    {
        this.radius = startRadius;
        this.maxRadius = maxRadius;
    }

    public void SetRadius(float r)
    {
        radius = Mathf.Clamp(r, 0.1f, maxRadius);
    }

    public void SetAttackFlash()
    {
        flash = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = flash ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}