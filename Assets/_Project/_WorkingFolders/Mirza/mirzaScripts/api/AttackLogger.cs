using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AttackLogger : MonoBehaviour
{
    public static AttackLogger Instance { get; private set; }

    private Dictionary<string, Dictionary<string, int>> attackData = new Dictionary<string, Dictionary<string, int>>();

    private void Awake()
    {
        // Singleton: Yalnýzca bir kopya kalsýn
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // fazlalýk varsa sil
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Sahne deðiþince silinmesin
    }

    public void AddAttack(string attacker, string skill, int damage)
    {
        if (!attackData.ContainsKey(attacker))
        {
            attackData[attacker] = new Dictionary<string, int>();
        }

        if (attackData[attacker].ContainsKey(skill))
        {
            attackData[attacker][skill] += damage;
        }
        else
        {
            attackData[attacker][skill] = damage;
        }
    }

    public string GetFormattedAttackData()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var attacker in attackData)
        {
            sb.AppendLine($"Saldýran: {attacker.Key}");
            foreach (var skill in attacker.Value)
            {
                sb.AppendLine($"  - Yetenek: {skill.Key}, Toplam Hasar: {skill.Value}");
            }
        }

        return sb.ToString();
    }
}
