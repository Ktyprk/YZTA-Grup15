using UnityEngine;
using System.Collections;
public class BossRangedAttackBehavior : IBossEnemyAttackBehavior
{
    private GameObject projectilePrefab;
    private float projectileSpeed;

    public BossRangedAttackBehavior(GameObject prefab, float speed)
    {
        projectilePrefab = prefab;
        projectileSpeed = speed;
    }

    public void Attack(BossEnemyController enemy, Transform target)
    {
        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        FireBallDamage fireBallDamage = proj.GetComponent<FireBallDamage>();
        fireBallDamage.isSummonByBoss = true;
        Vector3 targetPos = target.position;
        targetPos.y = spawnPos.y;
        Vector3 dir = (targetPos - spawnPos).normalized;
        proj.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;
       
    }
}

