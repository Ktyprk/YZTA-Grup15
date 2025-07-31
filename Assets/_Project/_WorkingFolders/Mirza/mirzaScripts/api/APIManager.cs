using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
    [Tooltip("Çalýþan /exec URL'ini buraya yapýþtýrýn.")]
    [SerializeField] private string gasURL;

    [Tooltip("Inspector'dan test etmek için bir soru yazýn.")]
    [SerializeField] private string prompt;
    private void Awake()
    {
        // Singleton: Yalnýzca bir kopya kalsýn
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // fazlalýk varsa sil
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Sahne deðiþince silinmesin
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Göndermeden önce prompt'un baþýndaki ve sonundaki boþluklarý sil
            /* string promptToSend = prompt.Trim();

             if (string.IsNullOrEmpty(promptToSend))
             {
                 Debug.LogError("PROMPT BOÞ! Lütfen Inspector'da bir soru yazýn ve tekrar deneyin.");
                 return; // Boþ istek gönderme
             }*/
            string header = "Bu veriler bana hasar veren düþmanlar, saldýrý isimleri ve toplam hasarlarý gösteriyor. " +
                 "Lütfen bu datalarý inceleyip bana hangi karakter ne kadar hasar vermiþ ve hangilerine daha çok dikkat etmem gerektiðini açýklar mýsýn(yakýn bir arkadýnmýs gibi konusursan sevinirim mantar þeklinde büyülü sevimli bir dost gibi düþün kendini ve cevap verirken öyle yaz emoji kullanma ve daha insansi içten bir arkadaþ gibi konus)?\n\n";

            string promptToSend = header + AttackLogger.Instance.GetFormattedAttackData().Trim();

            if (string.IsNullOrEmpty(promptToSend))
            {
                Debug.LogError("PROMPT BOÞ! Gönderilecek saldýrý verisi bulunamadý.");
                return;
            }

            StartCoroutine(SendDataToGAS(promptToSend));
        }
    }

    private IEnumerator SendDataToGAS(string finalPrompt)
    {
        // KONTROL 1: Gönderilen veriyi tam olarak görelim
        Debug.Log($"<color=yellow>ÝSTEK GÖNDERÝLÝYOR...</color>\nURL: {gasURL}\nSoru: '{finalPrompt}'");

        WWWForm form = new WWWForm();
        form.AddField("parameter", finalPrompt);

        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"<color=green>BAÞARILI CEVAP:</color>\n{www.downloadHandler.text}");
        }
        else
        {
            // KONTROL 2: Gelen hata mesajýný tam olarak görelim
            Debug.LogError($"<color=red>HATA GELDÝ:</color>\nKod: {www.responseCode}\nMesaj: {www.downloadHandler.text}");
        }
    }
}