using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour, IAttackType
{
    private enum CardinalDirection { Right, Left, Up, Down }

    private const float ParryWindow = .9f;
    private const float ParryCooldown = 1.0f;
    private const float ParryRadius = 1.2f;
    private const float ParryIframes = 0.6f;

    // Only parry attacks that come from within this cone in front of the player
    private const float ParryFrontCone = 60f;
    public float Cooldown => 0.5f;

    public void Attack(Player player)
    {
        Debug.Log("[SwordAttack] Normal attack executed");

        Vector3 spawnPosition = player.transform.position + (Vector3)player.facingDirection * .5f;

        GameObject swordHolder = Instantiate(
            player.swordPrefabAttack,
            spawnPosition,
            Quaternion.identity
        );

        float angle = Mathf.Atan2(player.facingDirection.y, player.facingDirection.x) * Mathf.Rad2Deg;
        swordHolder.transform.rotation = Quaternion.Euler(0, 0, angle);
        swordHolder.transform.parent = player.transform;

        Destroy(swordHolder, 0.3f);

        var enemies = Physics2D.OverlapCircleAll(
            player.transform.position,
            1.5f,
            LayerMask.GetMask("Enemy")
        );

        var projectiles = Physics2D.OverlapCircleAll(
            player.transform.position,
            1.5f,
            LayerMask.GetMask("Projectile")
        );
        Debug.Log($"[SwordAttack] Found {projectiles.Length} projectiles in range");

        foreach (var proj in projectiles)
        {
            Debug.Log($"[SwordAttack] Deflecting projectile {proj.name}");
            Destroy(proj.gameObject);
        }

        Debug.Log($"[SwordAttack] Found {enemies.Length} enemies in range");
        if (enemies.Length == 0) player.audioSource.PlayOneShot(player.swordSwingNothingSound);
        else player.audioSource.PlayOneShot(player.swordSwingEnemySound);

        foreach (var enemy in enemies)
        {
            Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)player.transform.position;
            float enemyAngle = Vector2.SignedAngle(player.facingDirection, toEnemy.normalized);
            Debug.Log($"[SwordAttack] Enemy {enemy.name} at angle {enemyAngle} (facing: {player.facingDirection})");

            if (enemyAngle >= -67.5f && enemyAngle <= 67.5f)
            {
                Debug.Log($"[SwordAttack] Attacking {enemy.name} with 1 damage");
                enemy.SendMessage("TakeDamage", player.attackDamage, SendMessageOptions.DontRequireReceiver);
                enemy.SendMessage("Knockback", toEnemy.normalized * 2f, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void heavyAttack(Player player)
    {
        Debug.Log($"[SwordAttack] Heavy attack initiated. Last heavy attack: {player.lastHeavyAttackTime}, Current time: {Time.time}, Cooldown: {ParryCooldown}");

        if (Time.time < player.lastHeavyAttackTime + ParryCooldown)
        {
            Debug.Log("[SwordAttack] Heavy attack on cooldown - aborting");
            return;
        }

        Debug.Log("[SwordAttack] Starting parry coroutine");
        player.audioSource.PlayOneShot(player.parrySound);
        player.StartCoroutine(ParryCoroutine(player));
    }

    private IEnumerator ParryCoroutine(Player player)
    {
        player.isHeavyAttacking = true;
        player.lastHeavyAttackTime = Time.time;
        player.SetParryVisual(true);
        player.SetInvulnerable(true);
        bool parrySuccess = false;

        // Create parry visual
        if (player.parryDirectionSprites != null && player.parryDirectionSprites.Length == 8)
        {
            Vector2 dir = player.facingDirection.normalized;
            int dirIndex = 0;
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

            Vector3 spawnPosition = player.transform.position + (Vector3)player.facingDirection * 0.5f;
            GameObject parrySprite = new GameObject("ParrySprite");
            parrySprite.transform.position = spawnPosition;
            parrySprite.transform.parent = player.transform;
            SpriteRenderer spriteRenderer = parrySprite.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = player.parryDirectionSprites[dirIndex];
            spriteRenderer.sortingLayerName = "Player";
            spriteRenderer.sortingOrder = (dirIndex == 1 || dirIndex == 2 || dirIndex == 3) ? -1 : 1;
            Destroy(parrySprite, ParryWindow + 0.1f);
        }

        float parryTimer = 0f;
        bool frontEnemyWasAttacking = false;

        while (parryTimer < ParryWindow)
        {
            parryTimer += Time.deltaTime;
            var enemies = Physics2D.OverlapCircleAll(
                player.transform.position,
                ParryRadius,
                LayerMask.GetMask("Enemy")
            );

            foreach (var enemy in enemies)
            {
                GoblinEnemy goblin = enemy.GetComponent<GoblinEnemy>();
                OgerMiniBoss ogre = enemy.GetComponent<OgerMiniBoss>();

                if (goblin == null && ogre == null) continue;

                Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)player.transform.position;
                float angle = Vector2.SignedAngle(player.facingDirection, toEnemy.normalized);
                bool isInFront = Mathf.Abs(angle) <= ParryFrontCone;

                Debug.Log($"Enemy: {enemy.name}, Angle: {angle}, Is in front: {isInFront}");

                // Check if the enemy is attacking
                bool isAttacking = false;
                if (goblin != null && goblin.currentState == GoblinEnemy.State.Attacking)
                {
                    isAttacking = true;
                }
                else if (ogre != null && ogre.currentState == OgerMiniBoss.State.Attacking)
                {
                    isAttacking = true;
                }

                if (isInFront && isAttacking)
                {
                    frontEnemyWasAttacking = true;
                    Debug.Log($"Enemy {enemy.name} is attacking and in front!");
                }
            }

            if (player.WasAttackedThisFrame() && frontEnemyWasAttacking)
            {
                parrySuccess = true;
                Debug.Log("[ParryCoroutine] PARRY SUCCESS! Enemy in front was attacking and player was hit.");
                break;
            }

            yield return null;
        }

        if (parrySuccess)
        {
            Debug.Log("[ParryCoroutine] Parry success: performing directed counter (normal attack)");
            yield return player.StartCoroutine(CounterFreezeFrames(0.2f));
            player.attackDamage *= 2;
            Attack(player);
            player.attackDamage /= 2;
            yield return new WaitForSeconds(ParryIframes);
        }
        else
        {
            Debug.Log("[ParryCoroutine] PARRY FAILED - Either no enemy in front was attacking or player wasn't hit.");
        }

        player.SetParryVisual(false);
        player.SetInvulnerable(false);
        player.isHeavyAttacking = false;
    }


    private IEnumerator CounterFreezeFrames(float duration)
    {
        Debug.Log("[CounterFreezeFrames] Starting freeze frame effect");
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Debug.Log("[CounterFreezeFrames] Freeze frame effect ended");
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ParryRadius);
        }
    }
}