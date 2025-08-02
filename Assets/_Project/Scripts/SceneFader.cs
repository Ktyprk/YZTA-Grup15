using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public float customDuration = 1f;
    public static SceneFader Instance;
    public int sceneIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeThenExecute(float duration, Action onFadeComplete)
    {
        image = GameObject.Find("FadeImage")?.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("FadeImage bulunamadı!");
            onFadeComplete?.Invoke();
            return;
        }

        StartCoroutine(FadeRoutine(duration, onFadeComplete));
    }

    private IEnumerator FadeRoutine(float duration, Action onFadeComplete)
    {
        yield return new WaitForEndOfFrame(); // küçük bir gecikme, yeni sahne geçişi varsa uyumsuzluk yaşanmasın

        // FADE-IN
        float t = 0f;
        Color color = image.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(t / duration);
            image.color = color;
            yield return null;
        }

        // Callback ile sahne yüklemesi yapılır
        onFadeComplete?.Invoke();

        // 1 frame sahne yüklenmesini bekleyip FADE-OUT başlatıyoruz
        yield return new WaitForSeconds(0.1f);

        // FADE-OUT
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (t / duration));
            image.color = color;
            yield return null;
        }

        // Tam sıfırla
        color.a = 0f;
        image.color = color;
    }
}