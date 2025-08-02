using UnityEngine;

public class LevelGate : MonoBehaviour
{
    [Header("Hangi sahne yüklensin")]
    public string sceneToLoad;

    [Header("Spawn noktası ID")]
    public string targetSpawnId;

    [Header("Geçişte fade efekti kullanılsın mı?")]
    public bool useFade = true;

    [Header("Fade süresi")]
    public float fadeDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (useFade && SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeThenExecute(fadeDuration, () =>
            {
                NewSceneLoader.Instance.LoadSceneWithPlayer(sceneToLoad, targetSpawnId);
            });
        }
        else
        {
            NewSceneLoader.Instance.LoadSceneWithPlayer(sceneToLoad, targetSpawnId);
        }
    }
}