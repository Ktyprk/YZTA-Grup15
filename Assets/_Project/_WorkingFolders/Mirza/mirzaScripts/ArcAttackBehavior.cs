using UnityEngine;

public class ArcAttackBehavior : IEnemyAttackBehavior
{
    private GameObject projectilePrefab;
    private float projectileSpeed;
    private float Shotingangle = 45f;

    public ArcAttackBehavior(GameObject prefab, float speed)
    {
        projectilePrefab = prefab;
        projectileSpeed = speed;
    }

    public void Attack(EnemyController enemy, Transform target)
    {
        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        Vector3 targetPos = target.position;
        Vector3 dir = targetPos - spawnPos;
        float h = dir.y;
        dir.y = 0;
        float distance = dir.magnitude;
        float radAngle = Shotingangle * Mathf.Deg2Rad;
        float velocityMagnitude = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * radAngle));
        float vxz = velocityMagnitude * Mathf.Cos(radAngle);
        float vy = velocityMagnitude * Mathf.Sin(radAngle);

        Vector3 result = dir.normalized * vxz;
        result.y = vy;
        rb.linearVelocity = result;
    }
}
