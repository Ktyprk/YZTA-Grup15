using UnityEngine;

public class SceneButtonLoader : MonoBehaviour
{
    [Header("Yüklenecek sahne adı")]
    public string sceneName;

    [Header("Fade süresi")]
    public float fadeDuration = 1f;

    [Header("Spawn ID (isteğe bağlı)")]
    public string spawnId = "StartPoint";
    
    public GameObject player;

    public void LoadSceneWithFade()
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeThenExecute(fadeDuration, () =>
            {
                Instantiate(player, Vector3.zero, Quaternion.identity);
                NewSceneLoader.Instance.LoadSceneWithPlayer(sceneName, spawnId);
                
            });
        }
        else
        {
            // fallback
            NewSceneLoader.Instance.LoadSceneWithPlayer(sceneName, spawnId);
        }
    }
}
