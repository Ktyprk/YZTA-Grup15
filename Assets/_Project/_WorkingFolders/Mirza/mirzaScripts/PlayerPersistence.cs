using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Aynı karakterden 2 tane olmasın
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    } }
