using UnityEngine;
using System.Collections;
public class damageColliderExplosiveBarrel : MonoBehaviour, IProjectileDamageDealer
{
    [SerializeField] private int damageAmount = 100;
   

    void Start()
    {

        gameObject.GetComponent<Collider>().enabled = true;
        StartCoroutine(GrowAndDestroyRoutine());
    }

    private IEnumerator GrowAndDestroyRoutine()
    {
        yield return new WaitForSeconds(1f);

        gameObject.GetComponent<Collider>().enabled = false;
         Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if(Icombat!=null)
            {
                Debug.Log("patladik");
               StartCoroutine( giveDamage(Icombat));
            }
           
           
        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        Icombat.TakeDamage(damageAmount);
        yield break;
    }

}
