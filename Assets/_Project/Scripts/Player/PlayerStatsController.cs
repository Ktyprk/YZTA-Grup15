using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float playerminDamage = 5f;
    public float playermaxDamage = 5f;
    public int playerArmor = 0;
    public int playerSpeed = 5;
    private Dictionary<buffType,float> stackStats = new Dictionary<buffType,float>();

    [HideInInspector] public int currentHealth;

    private float bonusAttack = 0;
    private int bonusMaxHealth = 0;
    private int bonusArmor = 0;
    private float bonusSpeed = 0f;

    [Header("UI Elements")]
    public Image healthBarFill;
    public TMP_Text healthText;

    public float Attack => playerminDamage + bonusAttack;
    public int MaxHealth => maxHealth + bonusMaxHealth;
    public int MaxArmor => playerArmor + bonusArmor;
    public float Speed => playerSpeed + bonusSpeed;

    private void Awake()
    {
        currentHealth = MaxHealth;
        UpdateHealthBar();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "StartPoint") // Sadece StartPoint sahnesinde çalýþsýn
        {
            deActiveBuff();
        } // Yeni sahne yüklenince tekrar çalýþýr
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            if (currentHealth > 0)
            {
                healthBarFill.fillAmount = (float)currentHealth / MaxHealth;

                healthText.text = currentHealth.ToString();
            }
            else
            {
                healthBarFill.fillAmount = 0;

                healthText.text = currentHealth.ToString();
            }
                
        }
        else
            Debug.LogError("HealthBarFill Image not assigned!");
    }

    public void ApplyBuff(buffCardsStats stats)
    {
       

        if (stats.type == buffType.Damage)
        {
            bonusAttack += stats.BoostplayermaxDamage;
            if (stackStats.ContainsKey(stats.type))
            {
                stackStats[stats.type] += stats.BoostplayermaxDamage; 
            }
            else
            {
                stackStats.Add(stats.type, stats.BoostplayermaxDamage); 
            }

        }
        else if (stats.type == buffType.MaxHealth)
        {
            bonusMaxHealth += stats.BoostmaxHealth;
            
            if (stackStats.ContainsKey(stats.type))
            {
                stackStats[stats.type] += stats.BoostmaxHealth;
            }
            else
            {
                stackStats.Add(stats.type, stats.BoostmaxHealth);
            }
        }
           
        else if (stats.type == buffType.Armor)
        {
            bonusArmor += stats.BoostplayerArmor;
            
            if (stackStats.ContainsKey(stats.type))
            {
                stackStats[stats.type] += stats.BoostplayerArmor;
            }
            else
            {
                stackStats.Add(stats.type, stats.BoostplayerArmor);
            }
        }
            
        else if (stats.type == buffType.Speed)
        {
            bonusSpeed += stats.BoostplayerSpeed;
            
            if (stackStats.ContainsKey(stats.type))
            {
                stackStats[stats.type] += stats.BoostplayerSpeed;
            }
            else
            {
                stackStats.Add(stats.type, stats.BoostplayerSpeed);
            }
        }
           

        currentHealth = Mathf.Min(currentHealth, MaxHealth);
        UpdateHealthBar();
    }
    public void deActiveBuff()
    {
        if(stackStats !=null)
        {
            foreach (var item in stackStats)
            {

                if (item.Key == buffType.Damage)
                {
                    bonusAttack -= item.Value;

                }
                else if (item.Key == buffType.MaxHealth)
                {
                    bonusMaxHealth -= (int)item.Value;
                }

                else if (item.Key == buffType.Armor)
                {
                    bonusArmor -= (int)item.Value;
                }

                else if (item.Key == buffType.Speed)
                {
                    bonusSpeed -= item.Value;
                }
            }
            stackStats.Clear();
        }
    


    

    }
    public void ApplyBuffMarket(marketCardsStats stats)
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