using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

using UnityEngine;
using TMPro;

public class DamagePopUpGenerator : MonoBehaviour
{
    public static DamagePopUpGenerator instance;

    public GameObject popUpPrefab; // Prefab'ı inspector'dan ata
    private Camera cam;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        cam = Camera.main;

        if (cam == null)
            Debug.LogError("Main Camera bulunamadı! Kameranı 'MainCamera' tag'iyle işaretle.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePopUp(Vector3.one * 2, Random.Range(23, 30).ToString());
        }
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        GameObject popUp = Instantiate(popUpPrefab, position, Quaternion.identity);
        
        Canvas canvas = popUp.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = cam;
        }
        
        TextMeshProUGUI tmp = popUp.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = text;
    }
}

