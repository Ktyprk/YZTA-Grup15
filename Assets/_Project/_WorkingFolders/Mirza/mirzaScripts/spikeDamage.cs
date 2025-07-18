using UnityEngine;
using System.Collections;

public class spikeDamage : MonoBehaviour, IProjectileDamageDealer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    [SerializeField] private int damageAmount;
    private float timer = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("hasarlandýk");
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                Debug.Log("hasarlandýk");
                StartCoroutine( giveDamage(Icombat));
            }
              

        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        Debug.Log("damage = " + damageAmount);
        Icombat.TakeDamage(damageAmount);
        yield break;
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player" )
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            timer += Time.deltaTime;
            if (timer > 2f && Icombat != null)
            {
                timer = 0;
                giveDamage(Icombat);
            }
        }

           
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
                timer = 0;
        }
    }
}
