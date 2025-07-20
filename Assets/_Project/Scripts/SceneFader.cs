using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public float customDuration = 1f;

    public void AAAAA()
    {
        Debug.Log("AAAAA");
    }
    public void FadeAndLoad(string sceneName)
    {
        Debug.Log("aaaa");
        //StartCoroutine(FadeInAndLoad(sceneName, customDuration));
        FadeAndLoadWithDuration(sceneName, customDuration);
    }

    public void FadeAndLoadWithDuration(string sceneName, float fadeDuration)
    {
        StartCoroutine(FadeInAndLoad(sceneName, fadeDuration));
    }

    IEnumerator FadeInAndLoad(string sceneName, float fadeDuration)
    {
        float t = 0;
        Color color = image.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = t / fadeDuration;
            image.color = color;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOutAndLoad(string sceneName, float fadeDuration)
    {
        float t = 0;
        Color color = image.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (t / fadeDuration));
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
