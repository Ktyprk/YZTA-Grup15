using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
