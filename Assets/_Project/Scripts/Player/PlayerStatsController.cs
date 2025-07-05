using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float playerDamage = 5f;
    public int playerArmor = 0;
    public int playerSpeed = 5;

    [HideInInspector] public int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(int amount)
    {
        currentHealth -= amount;
        return currentHealth <= 0;
    }
}
