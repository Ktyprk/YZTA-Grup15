using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public float customDuration = 1f;
    [SerializeField] private string nameOfScene;
    public static SceneFader Instance;
    public int sceneIndex = 0;

    void Awake()
    {
     
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sceneIndex = 0;
        }
        else
        {
           
            Destroy(gameObject);
        }
    }
    
    public void AAAAA()
    {
        Debug.Log("AAAAA");
    }
    public void FadeAndLoad(string sceneName)
    {
        if(sceneName=="MainMenu"||sceneName=="Mirza")
            sceneIndex = 0;
        //StartCoroutine(FadeInAndLoad(sceneName, customDuration));
        image = GameObject.Find("FadeImage").GetComponent<Image>();
        FadeAndLoadWithDuration(sceneName, customDuration);
    }

    public void FadeAndLoadWithDuration(string sceneName, float fadeDuration)
    {
        StartCoroutine(FadeInAndLoad(sceneName, fadeDuration));
    }

    IEnumerator FadeInAndLoad(string sceneName, float fadeDuration)
    {
         yield return new WaitForSeconds(0.1f);
        float t = 0;
        Color color = image.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = t / fadeDuration;
            image.color = color;
            yield return null;
        }
      
        color.a = 0;
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
    public void loadSceneWithTrigger()
    {

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "BoosRoom")
        {
            sceneIndex = 0;
            FadeAndLoad("StartPoint");
            return;
        }
        sceneIndex++;
        if (sceneIndex > RandomMapChooseManager.Instance.SceneEndIndex)
        {
            
            FadeAndLoad("BoosRoom");//mainhall it works when the last dungeon room complate succesfully and return mainhall
            
        }
        else
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(RandomMapChooseManager.Instance.shuffledNumbers[sceneIndex - 1]);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log("secene namee = " + sceneName);
            FadeAndLoad(sceneName);
        }
           
    }
    
}
