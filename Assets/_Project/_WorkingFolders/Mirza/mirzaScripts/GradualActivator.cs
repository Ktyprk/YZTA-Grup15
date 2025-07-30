using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradualActivator : MonoBehaviour
{
    [Header("Aktifleþecek Objeler")]
    public List<GameObject> objectsToActivate;

    [Header("Toplam Aktifleþtirme Süresi (saniye)")]
    public float totalDuration = 5f;

    private void Start()
    {
      //  StartCoroutine(ActivateObjectsGradually());
    }

    IEnumerator ActivateObjectsGradually()
    {
        int count = objectsToActivate.Count;

        
        float totalWeight = 0f;
        float[] weights = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = (float)(i + 1) / count; 
            float weight = Mathf.Pow(t, 2); 
            weights[i] = weight;
            totalWeight += weight;
        }

        for (int i = 0; i < count; i++)
        {
            if (objectsToActivate[i] != null)
                objectsToActivate[i].SetActive(true);

            // Normalleþtirilmiþ gecikme
            float delay = (weights[i] / totalWeight) * totalDuration;
            yield return new WaitForSeconds(delay);
        }
    }
}
