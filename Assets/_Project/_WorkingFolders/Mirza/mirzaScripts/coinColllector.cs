using UnityEngine;

public class coinColllector : MonoBehaviour
{

    public int coin;
    public static coinColllector Instance;
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            coin = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
