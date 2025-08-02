using UnityEngine;

public class IsoCameraFollow : MonoBehaviour
{
    public Transform target;          // Takip edilecek obje (örneğin oyuncu)
    public Vector3 offset = new Vector3(5f, 10f, -5f); // Kamera konumu (izometrik açı)
    public float smoothSpeed = 5f;    // Takip yumuşaklığı

    void LateUpdate()
    {
        if (target == null) return;

        // Hedef pozisyonu hesapla
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // Hedefe bakmaya devam et
        transform.LookAt(target);
    }
}