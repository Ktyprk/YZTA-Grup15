using UnityEngine;

public class RangedAttackBehavior : IEnemyAttackBehavior
{
    private GameObject projectilePrefab;
    private float projectileSpeed;

    public RangedAttackBehavior(GameObject prefab, float speed)
    {
        projectilePrefab = prefab;
        projectileSpeed = speed;
    }

    public void Attack(EnemyController enemy, Transform target)
    {
        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Vector3 targetPos = target.position;
        targetPos.y = spawnPos.y;
        Vector3 dir = (targetPos - spawnPos).normalized;
        proj.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;
    }
}
