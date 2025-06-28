using System.Collections.Generic;
using UnityEngine;
public class EntityPoolManager : MonoBehaviour
{
        public static EntityPoolManager Instance;
        
        public Dictionary<GameObject, GameObjectPool> EntityPools = new();
        [SerializeField] private Transform _entityParent;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private GameObjectPool GetEntityPool(GameObject prefab)
        {
            if (EntityPools.ContainsKey(prefab))
            {
                return EntityPools[prefab];
            }
            else
            {
                GameObjectPool pool = new GameObjectPool(prefab, _entityParent, 1);
                EntityPools.Add(prefab, pool);
                return pool;
            }
        }

        public GameObject SpawnEntity(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject entity = GetEntityPool(prefab).GetFromPool();
            
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            
            entity.SetActive(true);
            return entity;
        }
        
        public void ReleaseEntityToPool(GameObject prefab, GameObject entity)
        {
            entity.SetActive(false);
            GetEntityPool(prefab).PlaceToPool(entity);
        }
    }

