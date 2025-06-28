using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool : MonoBehaviour
{
    public GameObject Prefab;
    private Transform _poolParent;
    private List<GameObject> _pool = new List<GameObject>();
    
    public bool UseDisabledObjectStrategy;

    public GameObjectPool(GameObject prefab, Transform poolParent, int startingAmount)
    {
        Prefab = prefab;
        _poolParent = poolParent;

        for (var n = 0; n < startingAmount; n++)
        {
            CreateNewObject();
        }
    }

    public GameObject GetFromPool()
    {
        if (UseDisabledObjectStrategy)
        {
            foreach (GameObject obj in _pool)
            {
                if (obj == null)
                {
                    continue;
                }

                if (obj.activeSelf == false)
                {
                    return obj;
                }
            }

            return CreateNewObject();
        }
        else
        {
            GameObject gameObject;
            if (_pool.Count > 0)
            {
                gameObject = _pool[0];
            }
            else
            {
                gameObject = CreateNewObject();
            }

            _pool.Remove(gameObject);
            return gameObject;
        }
    }

    public GameObject CreateNewObject()
    {
        GameObject gameObject = GameObject.Instantiate(Prefab, _poolParent);
        gameObject.SetActive(false);
        _pool.Add(gameObject);

        return gameObject;
    }

    public void PlaceToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(_poolParent, true);

        _pool.Add(gameObject);
    }
}
