using UnityEngine;
using System.Collections;
public class damageColliderExplosiveBarrel : MonoBehaviour, IProjectileDamageDealer
{
    [SerializeField] private int damageAmount = 30;
   

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
        else if(other.gameObject.tag =="Enemy")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                Debug.Log("patladikEnemy");
                StartCoroutine(giveDamageVersionTwo(Icombat,other.gameObject));
            }
        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        PlayerController playerController= FindAnyObjectByType<PlayerController>();
        playerController.AddAttack("Explosive Barrel", "Explosion", damageAmount);
        Icombat.TakeDamage(damageAmount);
        yield break;
    }
    public IEnumerator giveDamageVersionTwo(ICombat Icombat,GameObject gameObject)
    {
        Debug.Log("ENEMYDAMAGE ");
        
        Icombat.TakeDamage(damageAmount);
        yield break;
    }

}
