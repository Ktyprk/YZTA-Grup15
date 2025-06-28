using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 3;
    [SerializeField] private float spawnRadius = 5f;

    [Header("Tekrar Spawn Engelle")]
    [SerializeField] private bool spawnOnlyOnce = true;
    private bool hasSpawned = false;
   
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (spawnOnlyOnce && hasSpawned) return;

        hasSpawned = true;

        Transform playerTransform = other.transform;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            
            GameObject enemyGO = EntityPoolManager.Instance.SpawnEntity(enemyPrefab, spawnPos, Quaternion.identity);
            EnemyController ec = enemyGO.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.SetTarget(playerTransform);
            }
        }
    }
    
    


    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 random = Random.insideUnitCircle * spawnRadius;
        return transform.position + new Vector3(random.x, 0, random.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
