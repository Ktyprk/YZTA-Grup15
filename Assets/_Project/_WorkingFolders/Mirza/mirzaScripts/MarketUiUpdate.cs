using TMPro;
using UnityEngine;
using static VTabs.VTabs;

public class MarketUiUpdate : MonoBehaviour
{
    public marketCardsStats marketCardsStats;
    [SerializeField] private PlayerStatsController playerStatsController;

    private CointText cointext;

    [Header("Ui Settings")]
    [SerializeField] private TMP_Text buffCost;
    [SerializeField] private TMP_Text buffName;
    [SerializeField] private TMP_Text buffInfo;

    [SerializeField] private GameObject MarketPanel;


    void Start()
    {
        cointext = FindAnyObjectByType<CointText>();
        buffCost.text = marketCardsStats.Cost.ToString();
        buffName.text = marketCardsStats.buffName.ToString();
        Debug.Log("isim = " + marketCardsStats.BoostplayerArmor.ToString());
        switch (marketCardsStats.type)
        {
            case buffType.MaxHealth:
                buffInfo.text = marketCardsStats.BuffInformation.ToString() + " " + marketCardsStats.BoostmaxHealth.ToString();
                break;
            case buffType.Damage:
                buffInfo.text = marketCardsStats.BuffInformation.ToString() + " " + marketCardsStats.BoostplayermaxDamage.ToString();
                break;
            case buffType.Speed:
                buffInfo.text = marketCardsStats.BuffInformation.ToString() + " " + marketCardsStats.BoostplayerSpeed.ToString();
                break;
            case buffType.Armor:
                buffInfo.text = marketCardsStats.BuffInformation.ToString() + " " + marketCardsStats.BoostplayerArmor.ToString();
                break;
            default:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
   
    public void applyBuff()
    {
        Debug.Log("hop");
        if(coinColllector.Instance.coin>= marketCardsStats.Cost)
        {
            Debug.Log("basildi");
            if (marketCardsStats != null)
                playerStatsController.ApplyBuffMarket(marketCardsStats);
            coinColllector.Instance.coin = coinColllector.Instance.coin- marketCardsStats.Cost;
            cointext.updateText();


        }
        else
        {
            Debug.Log("paran yetmedi paran = " + coinColllector.Instance.coin); 
        }
       
    }
 
}
