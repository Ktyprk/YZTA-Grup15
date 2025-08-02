using System.Collections.Generic;
using UnityEngine;

public class UniqueRandomGenerator
{
    private List<int> numberPool;
    private int currentIndex = 0;

    public UniqueRandomGenerator( int max)
    {
        numberPool = new List<int>();
        for (int i = 0; i < max; i++)
        {
            numberPool.Add(i);
        }

        Shuffle(numberPool);
    }

    public int GetUniqueRandom()
    {
        if (currentIndex >= numberPool.Count)
        {
            Debug.LogWarning("Tüm sayýlar tükendi!");
            return -1; // Ya da baþka bir hata durumu
        }

        int value = numberPool[currentIndex];
        currentIndex++;
        return value;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
