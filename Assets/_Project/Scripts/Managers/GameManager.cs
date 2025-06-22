using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject PlayerObject;

    public System.Action OnPlayerSpawned;
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayer(GameObject player)
    {
        PlayerObject = player;
        
        OnPlayerSpawned?.Invoke();
    }
}
