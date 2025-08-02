using UnityEngine;
using System.Collections;

public class CollectCoinWithTrigger : MonoBehaviour
{
    private CointText cointext;
    private void Start()
    {
        cointext = FindAnyObjectByType<CointText>();    
        StartCoroutine(movement());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag =="Player")
        {

            coinColllector.Instance.coin++;
            cointext.updateText();
            Destroy(gameObject,0.1f);
        }
    }
    public IEnumerator movement()
    {

        float timer = 0;
        bool goingUp = true;
        while (true)
        {
            timer += Time.deltaTime;

            if (goingUp)
            {
                transform.position += Vector3.up * Time.deltaTime;
            }
            else
            {
                transform.position -= Vector3.up * Time.deltaTime;
            }

            if (timer > 1f)
            {
                // Yön deðiþtir
                goingUp = !goingUp;
                timer = 0f;
            }


            yield return null;
        }
       
    }
}
