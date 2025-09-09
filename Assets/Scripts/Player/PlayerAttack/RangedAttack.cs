using UnityEngine;

public class RangedAttack : IAttackType
{
    public float Cooldown => 0.7f;

    public void Attack(Player player)
    {
        Debug.Log("Ranged Attack!");

        Transform firePoint = player.firePoint;
        GameObject projectilePrefab = player.projectilePrefab;

        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // Ensures z is 0 cause 2D
        Vector2 direction = ((Vector2)(mouseWorldPos - firePoint.position)).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (firePoint != null)
            {
                GameObject laserObj = new GameObject("LaserBeam");
                var lineRenderer = laserObj.AddComponent<LineRenderer>();
                // Configure lineRenderer (material, width, color, etc.)
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.colorGradient = new Gradient
                {
                    colorKeys = new GradientColorKey[]
                    {
                        new GradientColorKey(Color.red, 0f),
                        new GradientColorKey(Color.yellow, 1f)
                    },
                    alphaKeys = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 1f)
                    }
                };
                var laser = laserObj.AddComponent<LaserBeam>();
                laser.lineRenderer = lineRenderer;
                laser.bounceMask = LayerMask.GetMask("Player", "Wall"); // Set your wall layer
                Vector2 mouseWorldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 firePointPos2D = new Vector2(firePoint.position.x, firePoint.position.y);
                Vector2 direction2 = (mouseWorldPos2 - firePointPos2D).normalized;
                laser.Initialize(firePoint.position, direction2);
            }
        }
        else
        {
            // Normal attack
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * 15f; // Adjust speed as needed
                }
            }
        }
    }

    public void heavyAttack(Player player)
    {
        Debug.Log("Heavy Ranged Attack!");
        // Heavy ranged attack could shoot multiple projectiles in a spread
        Transform firePoint = player.firePoint;
        GameObject projectilePrefab = player.projectilePrefab;
        if (projectilePrefab != null && firePoint != null)
        {
            int projectileCount = 5;
            float spreadAngle = 30f; // Total spread angle in degrees
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // Ensures z is 0 cause 2D
            Vector2 baseDirection = ((Vector2)(mouseWorldPos - firePoint.position)).normalized;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = -spreadAngle / 2 + (spreadAngle / (projectileCount - 1)) * i;
                Vector2 direction = Quaternion.Euler(0, 0, angle) * baseDirection;
                GameObject projectile = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * 12f; // Slightly slower for heavy attack
                }
            }
        }
    }
}