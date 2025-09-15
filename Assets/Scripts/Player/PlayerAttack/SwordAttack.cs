// Updated SwordAttack.cs with extensive debugging
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwordAttack : IAttackType
{
    private const float ParryWindow = 0.6f;
    private const float ParryCooldown = 1.0f;
    private const float ParryRadius = 1.2f;
    private const float ParryIframes = 0.6f;

    public float Cooldown => 0.5f;

    public void Attack(Player player)
    {
        Debug.Log("[SwordAttack] Normal attack executed");
        var enemies = Physics2D.OverlapCircleAll(
            player.transform.position,
            1.5f,
            LayerMask.GetMask("Enemy")
        );

        Debug.Log($"[SwordAttack] Found {enemies.Length} enemies in range");

        foreach (var enemy in enemies)
        {
            Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)player.transform.position;
            float angle = Vector2.SignedAngle(player.facingDirection, toEnemy.normalized);

            Debug.Log($"[SwordAttack] Enemy {enemy.name} at angle {angle}° (facing: {player.facingDirection})");

            if (angle >= -60f && angle <= 60f)
            {
                Debug.Log($"[SwordAttack] Attacking {enemy.name} with 3 damage");
                enemy.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
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
        player.StartCoroutine(ParryCoroutine(player));
    }

    private IEnumerator ParryCoroutine(Player player)
    {
        player.isHeavyAttacking = true;
        player.lastHeavyAttackTime = Time.time;
        player.SetParryVisual(true);
        player.SetInvulnerable(true); // Make player invulnerable during parry attempt

        List<GameObject> attackingEnemies = new List<GameObject>();
        float parryTimer = 0f;
        bool parrySuccess = false;

        // Track if any enemy was in attack state during the window
        bool enemyWasAttacking = false;

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
                if (goblin != null && goblin.currentState == GoblinEnemy.State.Attacking)
                {
                    if (!attackingEnemies.Contains(enemy.gameObject))
                    {
                        attackingEnemies.Add(enemy.gameObject);
                        enemyWasAttacking = true; // Mark that an enemy was attacking
                    }
                }
            }

            // If an enemy was attacking AND the player was hit, it's a successful parry
            if (player.WasAttackedThisFrame() && enemyWasAttacking)
            {
                parrySuccess = true;
                Debug.Log("[ParryCoroutine] PARRY SUCCESS! Enemy was attacking and player was hit.");
                break;
            }

            yield return null;
        }

        if (parrySuccess)
        {
            Debug.Log("[ParryCoroutine] Executing counterattack on " + attackingEnemies.Count + " enemies");

            foreach (var enemy in attackingEnemies)
            {
                if (enemy != null)
                {
                    enemy.SendMessage("TakeDamage", 5, SendMessageOptions.DontRequireReceiver);
                    enemy.SendMessage("Stun", 0.5f, SendMessageOptions.DontRequireReceiver);
                }
            }

            yield return player.StartCoroutine(CounterFreezeFrames(0.1f));
            yield return new WaitForSeconds(ParryIframes);
        }
        else
        {
            Debug.Log("[ParryCoroutine] PARRY FAILED - Either no enemies were attacking or player wasn't hit.");
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
}
