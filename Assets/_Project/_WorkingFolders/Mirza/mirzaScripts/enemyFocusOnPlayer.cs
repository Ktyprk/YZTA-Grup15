using Unity.VisualScripting;
using UnityEngine;

public class enemyFocusOnPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BossEnemyController ec = FindAnyObjectByType<BossEnemyController>();
            if (ec != null) { ec.SetTarget(other.transform); }
            gameObject.GetComponent<Collider>().enabled = false;

        }
    }
}
