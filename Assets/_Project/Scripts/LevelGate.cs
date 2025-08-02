using UnityEngine;

public class LevelGate : MonoBehaviour
{
    [Header("Hangi sahne yüklensin")]
    public string sceneToLoad;

    [Header("Spawn noktası ID")]
    public string targetSpawnId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NewSceneLoader.Instance.LoadSceneWithPlayer(sceneToLoad, targetSpawnId);
        }
    }
}
