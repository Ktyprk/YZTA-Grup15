using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject spawnposition;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemySpawner enemySpawner;
    public int counter;
    void Start()
    {
        counter = 0;
    }

    
    void Update()
    {
        Debug.Log("SceneEndIndex = " + RandomMapChooseManager.Instance.SceneEndIndex + " " + "sceneIndex =  " + SceneFader.Instance.sceneIndex);
        if(counter>=4 && RandomMapChooseManager.Instance.SceneEndIndex  == SceneFader.Instance.sceneIndex)
        {
            counter = 0;
            enemySpawner.spawnBoss(enemyPrefab, spawnposition, player);
        }
    }
}
