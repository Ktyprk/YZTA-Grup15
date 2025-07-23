using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BossSpecialAttackBehavior : IBossEnemyAttackBehavior
{
    private GameObject projectilePrefab;
    private float projectileSpeed;
    private int burstCount;
    private float delayBetweenBursts;
    private float radius;
    private BossEnemyController bossEnemyController;

    private MonoBehaviour coroutineHost;
    EnemyAnimatorController enemyAnimatorController;

    public BossSpecialAttackBehavior(GameObject prefab, float speed, float radius,BossEnemyController bossEnemyController,EnemyAnimatorController enemyAnimatorController ,MonoBehaviour host)
    {
        projectilePrefab = prefab;
        projectileSpeed = speed;
       
        this.radius = radius;
        this.bossEnemyController = bossEnemyController;
        this.enemyAnimatorController = enemyAnimatorController;
        coroutineHost = host;
    }

    public void Attack(BossEnemyController enemy, Transform target)
    {
        bossEnemyController.SpecialAttack = true;

        coroutineHost.StartCoroutine(ShootBurst(enemy.transform));
    }

    private IEnumerator ShootBurst(Transform character)
    {
       

        
           
            ShootIn8Directions(character, bossEnemyController.angleOffset);
           
        yield return null;
        
       
    }

    private void ShootIn8Directions(Transform character, float offset)
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f + offset;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 spawnPos = character.position + dir * radius + Vector3.up * 1f;

            GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
            proj.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;
        }
    }
}
