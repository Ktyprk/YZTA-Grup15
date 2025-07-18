using UnityEngine;
using System.Collections;

public class BladeDamage : MonoBehaviour, IProjectileDamageDealer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    [SerializeField] private int damageAmount;
    private float timer = 0;
    public void OnTriggerEnter(Collider other)
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("hasarlandýk");
            ICombat Icombat = collision.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                Debug.Log("hasarlandýk");
                StartCoroutine(giveDamage(Icombat));
            }


        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        Debug.Log("damage = " + damageAmount);
        Icombat.TakeDamage(damageAmount);
        yield break;
    }
    

}
