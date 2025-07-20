using UnityEngine;
using System.Collections;

public class FireBallDamage : MonoBehaviour , IProjectileDamageDealer
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private int fireDamageAmount = 5;

    public void Start()
    {
        Destroy(gameObject, 4);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                StartCoroutine(giveDamage(Icombat));

            }
        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        Icombat.TakeDamage(damageAmount);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            Icombat.TakeDamage(fireDamageAmount);
            Debug.Log("damage Verildi");
            yield return new WaitForSeconds(1f);

        }
        Destroy(gameObject);
    }
}
