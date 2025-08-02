using UnityEditor.TerrainTools;
using UnityEngine;


[CreateAssetMenu(menuName = "MarketCard/Create New Buff Data")]

public class marketCardsStats : ScriptableObject
{
    public buffType type;
    public int BoostmaxHealth = 0;
    public float BoostplayerminDamage = 0f;
    public float BoostplayermaxDamage = 0f;
    public int BoostplayerArmor = 0;
    public int BoostplayerSpeed = 0;
    public string buffName;
    public string BuffInformation;
    public int Cost = 0;
}
