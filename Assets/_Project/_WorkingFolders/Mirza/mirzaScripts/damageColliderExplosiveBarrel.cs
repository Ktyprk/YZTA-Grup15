using UnityEngine;
using System.Collections;
public class damageColliderExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private int damageAmount;
   

    void Start()
    {
     
        StartCoroutine(GrowAndDestroyRoutine());
    }

    private IEnumerator GrowAndDestroyRoutine()
    {
        yield return new WaitForSeconds(1f);

        gameObject.GetComponent<Collider>().enabled = false;
         Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {

                Icombat.TakeDamage(100);

            }



        }
    }

}
