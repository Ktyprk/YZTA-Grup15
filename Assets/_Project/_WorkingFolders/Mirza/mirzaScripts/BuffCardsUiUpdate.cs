using UnityEngine;
using TMPro;
using VFavorites.Libs;


public class BuffCardsUiUpdate : MonoBehaviour
{
    private buffCardsStats buffCardsStat;
    [SerializeField] private PlayerStatsController playerStatsController;
    [SerializeField] private GameObject buffShowTrigger;
   
    
    [Header("Ui Settings")]
    [SerializeField] private TMP_Text buffName;
    [SerializeField] private TMP_Text buffInfo;

    [SerializeField] private GameObject BuffPanel;


    void Start()
    {
        buffShowTrigger = GameObject.Find("BuffShowTrigger");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateTexts(buffCardsStats statinfo)
    {
        buffCardsStat = statinfo;
        buffName.text = buffCardsStat.buffName.ToString();
        Debug.Log("isim = " + buffCardsStat.BoostplayerArmor.ToString());
        switch (buffCardsStat.type)
        {
            case buffType.MaxHealth:
                buffInfo.text = buffCardsStat.BuffInformation.ToString() + " "+statinfo.BoostmaxHealth.ToString();
                break;
            case buffType.Damage:
                buffInfo.text = buffCardsStat.BuffInformation.ToString() + " " + statinfo.BoostplayermaxDamage.ToString();
                break;
            case buffType.Speed:
                buffInfo.text = buffCardsStat.BuffInformation.ToString() + " " + statinfo.BoostplayerSpeed.ToString();
                break;
            case buffType.Armor:
                buffInfo.text = buffCardsStat.BuffInformation.ToString() + " " + statinfo.BoostplayerArmor.ToString();
                break;
            default:
                break;
        }
    }
    public void applyBuff()
    {
        Debug.Log("basildi");
        if(buffCardsStat!=null)
        playerStatsController.ApplyBuff(buffCardsStat);
        BuffPanel.SetActive(false);
        buffShowTrigger.SetActive(false);
        
    }
}
