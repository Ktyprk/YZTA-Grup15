using UnityEngine;
using System.Collections;

public class MushroomColorChange : MonoBehaviour
{
    private Renderer rend;
    private Material[] materials;
    private Color[] originalColors;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject gameObjects;
    Vector3 transformLocation;

    public float flashInterval = 0.2f;
    public float totalFlashDuration = 1f;

    void Start()
    {
        transformLocation = transform.position;
        transformLocation.y += 2f;
        rend = GetComponent<Renderer>();
        materials = rend.materials; // Tüm materyalleri al

        // Her bir materialin orijinal rengini sakla
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public IEnumerator FlashForOneSecond()
    {
        float elapsed = 0f;

        while (elapsed < totalFlashDuration)
        {
            // Tüm materyalleri beyaza çevir
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.white;
            }

            yield return new WaitForSeconds(flashInterval / 2f);

            // Tüm materyalleri eski rengine döndür
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(flashInterval / 2f);

            elapsed += flashInterval;
        }

        Instantiate(smoke, transformLocation, Quaternion.identity);
        Destroy(gameObjects, 0.2f);
    }
}
