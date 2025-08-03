using UnityEngine;
using System.Collections;

public class MushroomColorChange : MonoBehaviour
{
    private Renderer rend;
    private Material[] materials;
    private Color[] originalColors;
    [SerializeField] private Material HealMushroom;
    [SerializeField] private Material DamageMushroom;
    [SerializeField] private Material SpeedMushroom;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject smokeDamageArea;
    [SerializeField] private GameObject gameObjects;
    Vector3 transformLocation;
    Vector3 transformLocation2;

    public float flashInterval = 0.2f;
    public float totalFlashDuration = 1f;

    void Start()
    {
        transformLocation = transform.position;
        transformLocation2 = transform.position;
        transformLocation.y += 1f;
     
    }

    public IEnumerator FlashForOneSecond(Vector3 spawnLocation,MushroomTypes type,int damage_Heal_Amount)
    {
        float elapsed = 0f;
        damageColliderOfSmoke damageColliderOfsmoke = smokeDamageArea.GetComponent<damageColliderOfSmoke>();
        damageColliderOfsmoke.damage_Heal_Amount = damage_Heal_Amount;
        damageColliderOfsmoke.mushroomTypes = type;
        while (elapsed < totalFlashDuration)
        {
            // Tüm materyalleri beyaza çevir
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.darkRed;
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


    GameObject smokes =    Instantiate(smoke, spawnLocation, Quaternion.identity);
        Instantiate(smokeDamageArea, spawnLocation, Quaternion.identity);
        SoundManager.Instance.PlayAudio("mushroom",0.5f);
        if (type == MushroomTypes.heal)
        {
            var main = smokes.GetComponent<ParticleSystem>().main;
            main.startColor = Color.green;
        }
        else if (type == MushroomTypes.damage)
        {
            var main = smokes.GetComponent<ParticleSystem>().main;
            main.startColor = Color.rebeccaPurple;
        }
        else if (type == MushroomTypes.speed)
        {
            
            var main = smokes.GetComponent<ParticleSystem>().main;
            main.startColor = Color.blue;
        }
        Destroy(gameObjects, 0.5f);
    }
    public void getMetarials(MushroomTypes type)
    {
        rend = GetComponent<Renderer>();
        if(type== MushroomTypes.heal)
        {
            rend.material = HealMushroom;
            var main = smoke.GetComponent<ParticleSystem>().main;
            main.startColor = Color.green;
        }
        else if(type == MushroomTypes.damage)
        {
            rend.material = DamageMushroom;
            var main = smoke.GetComponent<ParticleSystem>().main;
            main.startColor = Color.rebeccaPurple;
        }
        else if(type == MushroomTypes.speed)
        {
            rend.material = SpeedMushroom;
            var main = smoke.GetComponent<ParticleSystem>().main;
            main.startColor = Color.blue;
        }
        
        materials = rend.materials; // Tüm materyalleri al

        // Her bir materialin orijinal rengini sakla
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }
}
