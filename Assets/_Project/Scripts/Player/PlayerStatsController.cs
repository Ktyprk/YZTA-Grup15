using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float playerminDamage = 5f;
    public float playermaxDamage = 5f;
    public int playerArmor = 0;
    public int playerSpeed = 5;

    [HideInInspector] public int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
}
