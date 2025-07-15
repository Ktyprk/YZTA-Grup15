using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float playerminDamage = 5f;
    public float playermaxDamage = 5f;
    public int playerArmor = 0;
    public int playerSpeed = 5;

    [HideInInspector] public int currentHealth;

    private float bonusAttack = 0;
    private int bonusMaxHealth = 0;
    private int bonusArmor = 0;
    private float bonusSpeed = 0f;

    [Header("UI Elements")]
    public Image healthBarFill;

    public float Attack => playerminDamage + bonusAttack;
    public int MaxHealth => maxHealth + bonusMaxHealth;
    public int MaxArmor => playerArmor + bonusArmor;
    public float Speed => playerSpeed + bonusSpeed;

    private void Awake()
    {
        currentHealth = MaxHealth;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / MaxHealth;
        else
            Debug.LogError("HealthBarFill Image not assigned!");
    }

    public void ApplyBuff(buffCardsStats stats)
    {
        if (stats.type == buffType.Damage)
            bonusAttack += stats.BoostplayermaxDamage;
        else if (stats.type == buffType.MaxHealth)
            bonusMaxHealth += stats.BoostmaxHealth;
        else if (stats.type == buffType.Armor)
            bonusArmor += stats.BoostplayerArmor;
        else if (stats.type == buffType.Speed)
            bonusSpeed += stats.BoostplayerSpeed;

        currentHealth = Mathf.Min(currentHealth, MaxHealth);
        UpdateHealthBar();
    }
}