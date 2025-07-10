using UnityEngine;
using System.Collections;

public class damageColliderOfSmoke : MonoBehaviour , IProjectileDamageDealer
{
    public float growDuration = 1f;   
    public Vector3 targetScale = new Vector3(2f, 1f, 2f);

    void Start()
    {
        targetScale = new Vector3(5.3f, gameObject.transform.localScale.y, 5.3f);
        StartCoroutine(GrowAndDestroyRoutine());
    }

    private IEnumerator GrowAndDestroyRoutine()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;

           
            float newX = Mathf.Lerp(startScale.x, targetScale.x, t);
            float newZ = Mathf.Lerp(startScale.z, targetScale.z, t);

            transform.localScale = new Vector3(newX, startScale.y, newZ);

            yield return null;
        }

        gameObject.GetComponent<Collider>().enabled = false;
        //    Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag =="Player")
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
        for (int i = 0; i < 3; i++)
        {
            Icombat.TakeDamage(2);
            Debug.Log("damage Verildi");
            yield return new WaitForSeconds(1f);
            
        }
        Destroy(gameObject);
    }
}
