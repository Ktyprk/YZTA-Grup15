using UnityEngine;

public class RangedAttackBehavior : IEnemyAttackBehavior
{
    private GameObject projectilePrefab;
    private float projectileSpeed;
    EnemyData enemyData;
    public RangedAttackBehavior(EnemyData enemyData,GameObject prefab, float speed)
    {
        projectilePrefab = prefab;
        projectileSpeed = speed;
        this.enemyData = enemyData;
    }

    public void Attack( EnemyController enemy, Transform target)
    {
        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
        SoundManager.Instance.PlayAudio("fireball-01", 0.01f);
        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        FireBallDamage fireBallDamage = proj.GetComponent<FireBallDamage>();
        if ( fireBallDamage != null )
            fireBallDamage.enemyData = enemyData;
        Vector3 targetPos = target.position;
        targetPos.y = spawnPos.y;
        Vector3 dir = (targetPos - spawnPos).normalized;
        proj.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;
    }
}
