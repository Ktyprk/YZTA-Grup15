using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneLoader : MonoBehaviour
{
    public static NewSceneLoader Instance;
    private string desiredSpawnId;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadSceneWithPlayer(string sceneName, string spawnId)
    {
        desiredSpawnId = spawnId;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GameObject rootObject = player.transform.root.gameObject;
            DontDestroyOnLoad(rootObject); 

            // Önce hizala — Player alt nesne olduğu için local offset sıfırlanır
            AlignTransform(player.transform, rootObject.transform);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
       // SceneFader.Instance.loadSceneWithTrigger();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndAlign());
    }

    private IEnumerator WaitAndAlign()
    {
        yield return new WaitForEndOfFrame(); // Yüklenmenin ardından 1 frame bekle

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player bulunamadı! Spawn başarısız.");
            yield break;
        }

        GameObject rootObject = player.transform.root.gameObject;

        var spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
        foreach (var point in spawnPoints)
        {
            if (point.spawnId == desiredSpawnId)
            {
                rootObject.transform.SetParent(point.transform);
                rootObject.transform.localPosition = Vector3.zero;
                rootObject.transform.localRotation = Quaternion.identity;
                rootObject.transform.localScale = Vector3.one;
                rootObject.transform.SetParent(null, true);

                AlignTransform(player.transform, rootObject.transform);
                yield break;
            }
        }

        Debug.LogWarning("Eşleşen spawn point bulunamadı: " + desiredSpawnId);
    }


    // 🔁 Transform hizalama fonksiyonu
    private void AlignTransform(Transform target, Transform reference)
    {
        target.SetPositionAndRotation(reference.position, reference.rotation);
        target.localScale = reference.localScale;
    }
}
