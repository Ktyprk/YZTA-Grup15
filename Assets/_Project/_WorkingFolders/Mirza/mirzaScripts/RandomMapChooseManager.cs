using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapChooseManager : MonoBehaviour
{
     public int sceneStartIndex, SceneEndIndex;
    public static RandomMapChooseManager Instance;

    public List<int> shuffledNumbers = new List<int>();

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeList();             
        }
        else
        {
            Destroy(gameObject); 
        }
    }

   public void InitializeList()
    {
        shuffledNumbers.Clear();

      
        for (int i = sceneStartIndex; i <= SceneEndIndex; i++)
        {
            shuffledNumbers.Add(i);
        }

        ShuffleList(shuffledNumbers);
    }

   
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); 
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

   
    public void Reshuffle()
    {
        ShuffleList(shuffledNumbers);
    }
}
