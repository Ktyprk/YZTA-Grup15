using UnityEngine;
using System.Collections;

public interface IProjectileDamageDealer 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other);
    IEnumerator giveDamage(ICombat Icombat);


  
}
