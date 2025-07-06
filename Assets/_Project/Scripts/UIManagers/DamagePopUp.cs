using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeSpeed = 1f;
    public float lifeTime = 1f;

    private TextMeshProUGUI textMesh;
    private Color startColor;
    private float timer = 0f;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        startColor = textMesh.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        
        float alpha = Mathf.Lerp(startColor.a, 0f, timer / lifeTime);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        
        if (Camera.main != null)
        {
            Vector3 dir = (transform.position - Camera.main.transform.position).normalized;
            transform.forward = dir;
        }
        
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}