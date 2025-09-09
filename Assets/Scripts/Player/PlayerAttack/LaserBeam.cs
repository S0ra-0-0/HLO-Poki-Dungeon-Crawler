#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LaserBeam : MonoBehaviour
{
    public float duration = 2f;
    public float damageInterval = 1f;
    public float maxDistance = 20f;
    public int maxBounces = 3;
    public LayerMask bounceMask;
    public LineRenderer lineRenderer;

    private List<Collider2D> hitEnemies = new List<Collider2D>();
    private List<Vector3> lastBeamPoints = new List<Vector3>(); 

    public void Initialize(Vector2 origin, Vector2 direction)
    {
        StartCoroutine(FireBeam(origin, direction));
    }

    private IEnumerator FireBeam(Vector2 origin, Vector2 direction)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            DrawBeam(origin, direction);
            DealDamageAlongBeam();
            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }
        Destroy(gameObject);
    }

    private void DrawBeam(Vector2 origin, Vector2 direction)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(origin);
        Vector2 currentOrigin = origin;
        Vector2 currentDir = direction;
        int bounces = 0;
        Collider2D lastCollider = null;

        Debug.Log($"[LaserBeam] Starting DrawBeam. Origin: {origin}, Direction: {direction}, MaxBounces: {maxBounces}, MaxDistance: {maxDistance}, BounceMask: {bounceMask.value}");

        while (bounces < maxBounces)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDir, maxDistance, bounceMask);
            // Ignore colliders with the "Player" tag
            while (hit && hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Continue the ray from just past the ignored collider
                currentOrigin = hit.point + currentDir * 0.01f;
                hit = Physics2D.Raycast(currentOrigin, currentDir, maxDistance, bounceMask);
            }

            if (hit && hit.collider != null && hit.collider != lastCollider)
            {
                Debug.Log($"[LaserBeam] Bounce {bounces + 1}: Hit at {hit.point} with normal {hit.normal}, collider: {hit.collider.name}");
                points.Add(hit.point);
                lastCollider = hit.collider;
                currentOrigin = hit.point + hit.normal * 0.01f;
                currentDir = Vector2.Reflect(currentDir, hit.normal);
                bounces++;
            }
            else
            {
                Debug.Log($"[LaserBeam] No hit or repeated hit. Adding endpoint at {currentOrigin + currentDir * maxDistance}");
                points.Add(currentOrigin + currentDir * maxDistance);
                break;
            }
        }

        lastBeamPoints = points; // Store for Gizmos

        Debug.Log($"[LaserBeam] Total points in line: {points.Count}");
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
            Debug.Log($"[LaserBeam] LineRenderer point {i}: {points[i]}");
        }
    }

    private void DealDamageAlongBeam()
    {
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector2 start = lineRenderer.GetPosition(i);
            Vector2 end = lineRenderer.GetPosition(i + 1);
            RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    //// Replace with your enemy damage logic
                    //hit.collider.GetComponent<Enemy>()?.TakeDamage(1);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lastBeamPoints != null && lastBeamPoints.Count > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < lastBeamPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(lastBeamPoints[i], lastBeamPoints[i + 1]);
            }
        }
    }
#endif
}           
