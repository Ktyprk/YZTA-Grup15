using UnityEngine;

public class GameRoot : MonoBehaviour
{

    private static GameRoot instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
