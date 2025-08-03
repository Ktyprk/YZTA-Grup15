using UnityEngine;

public class CoinDropSystem : MonoBehaviour
{
    [SerializeField] private GameObject Coin;
    [SerializeField] private int dropRateBaseOnPercentage =40;
    [SerializeField] private bool isDrop = false; 
    void Start()
    {
        int random = Random.Range(0, 100);
        if(random<dropRateBaseOnPercentage)
        {
            isDrop = true;
        }
        else
        {
            isDrop= false;
        }
    }

    public void DropCoin()
    {
        if (isDrop)
        {
           GameObject coin = Instantiate(Coin,transform.position,Quaternion.identity);
        }

    }
}
