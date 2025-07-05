using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosiveBarrel : MonoBehaviour
{

    [SerializeField] private GameObject circleAroundBarrel;
    [SerializeField] private GameObject blowUpeffect;
    
    public void HealChange()
    {

        circleAroundBarrel.SetActive(true);
        StartCoroutine(blowupEffect());

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            StartCoroutine(blowupEffect());
        }
    }
    IEnumerator blowupEffect()
    {
        circleAroundBarrel.SetActive(true);
        yield return new WaitForSeconds(1);
        circleAroundBarrel.SetActive(false);

        yield return new WaitForSeconds(1);
        circleAroundBarrel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Instantiate(blowUpeffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
