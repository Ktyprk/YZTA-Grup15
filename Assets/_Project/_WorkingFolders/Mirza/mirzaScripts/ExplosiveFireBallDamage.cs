using UnityEngine;
using System.Collections;

public class ExplosiveFireBallDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private int fireDamageAmount = 5;
    [SerializeField] private GameObject bouncedObject;

    public void Start()
    {
        Destroy(gameObject, 4);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || LayerMask.LayerToName(other.gameObject.layer) == "Ground")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                StartCoroutine(giveDamage(Icombat));

            }
            else
            {
                BounceObject();
            }
        }
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        BounceObject();
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

    public void BounceObject()
    {
        Vector3[] targetPositions = new Vector3[3];
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        targetPositions[0]= transform.position + Vector3.forward*3;
        targetPositions[1] = transform.position + Vector3.right * 3;
        targetPositions[2] = transform.position + Vector3.left * 3;
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = gameObject.transform.position + Vector3.up * 1f;
            GameObject proj = GameObject.Instantiate(bouncedObject, spawnPos, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.useGravity = true;
            Vector3 dir = targetPositions[i] - spawnPos;
            float h = dir.y;
            dir.y = 0;
            float distance = dir.magnitude;
            float radAngle = 30f * Mathf.Deg2Rad;
            float velocityMagnitude = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * radAngle));
            float vxz = velocityMagnitude * Mathf.Cos(radAngle);
            float vy = velocityMagnitude * Mathf.Sin(radAngle);

            Vector3 result = dir.normalized * vxz;
            result.y = vy;
            rb.linearVelocity = result;
        }
       
     
    }
}
