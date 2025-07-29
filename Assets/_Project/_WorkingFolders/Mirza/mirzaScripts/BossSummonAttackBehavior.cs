using UnityEngine;
using System.Collections;

public class BossSummonAttackBehavior : IBossEnemyAttackBehavior
{

    private GameObject[] summonPrefab;
    private float summonCount;
    private Transform transformm;
    private float spawnRadius = 5f;
    private GameObject smoke;
    public BossSummonAttackBehavior(GameObject[] summonPrefab, GameObject smoke,float summonCount)
    {
        this.summonPrefab = summonPrefab;
        this.summonCount = summonCount;
        this.smoke = smoke;
    }

    public void Attack(BossEnemyController enemy, Transform target)
    {
        transformm = target;
        for (int i = 0; i < summonCount; i++)
        {
           int j = Random.Range(0,summonPrefab.Length);
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemyGO = EntityPoolManager.Instance.SpawnEntity(summonPrefab[j], spawnPos, Quaternion.identity);
            GameObject smokeSummon = Object.Instantiate(smoke, enemyGO.transform.position, Quaternion.identity);
            EnemyController ec = enemyGO.GetComponent<EnemyController>();
            ec.summonByBoss = true;
            Debug.Log("summonlandý");
            if (ec != null)
            {
                ec.SetTarget(target);
            }
            Object.Destroy(smokeSummon, 2f);
        }

    }
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 random = Random.insideUnitCircle * spawnRadius;
        return transformm.position + new Vector3(random.x, 0, random.y);
    }
}
