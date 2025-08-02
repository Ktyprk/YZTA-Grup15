using UnityEngine;
using System.Collections.Generic;

public class BuffCardsShowTrigger : MonoBehaviour
{
    [SerializeField] private buffCardsStats[] buffs;
    [SerializeField] private BuffCardsUiUpdate[] buffcards;
    [SerializeField] private GameObject buffMenu;

    [SerializeField] private UniqueRandomGenerator randomGenerateNumber;

    [SerializeField]  private int[] randomValues;
    public GameObject buffCardsParent;

 
    private void Start()
    {
        PlayerController controller = FindAnyObjectByType<PlayerController>();

        buffCardsParent = controller.BuffCards;
        buffMenu = controller.BuffCards;
        buffcards = buffCardsParent.GetComponentsInChildren<BuffCardsUiUpdate>();

        randomValues = new int[buffs.Length];

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            randomGenerateNumber = new UniqueRandomGenerator(buffs.Length); 

            for (int i = 0; i < buffs.Length; i++)
            {
                randomValues[i] = randomGenerateNumber.GetUniqueRandom();
                Debug.Log("Tekrarsýz sayý: " + randomValues[i]);
            }
            for (int i = 0; i < buffcards.Length; i++)
            {
                buffcards[i].updateTexts(buffs[randomValues[i]]);
            }
            buffMenu.SetActive(true);

        }
    }
    void Update()
    {
        
    }
}
