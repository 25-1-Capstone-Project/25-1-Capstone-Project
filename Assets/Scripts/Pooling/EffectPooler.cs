using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 오브젝트 풀링을 관리하는 스크립트입니다.
/// !!주의!! destoy는 사용하지 않고, 비활성화 시켜서 풀링
/// ObjectPooler.Instance.SpawnFromPool() 로 오브젝트 사용
/// OnDisable()=> ObjectPooler.Instance.ReturnToPool(gameObject) 를 호출하여 풀링된 오브젝트를 반환합니다.
/// effect의 경우 파티클 시스템에서 StopAction Disable 설정
/// </summary>
public class EffectPooler : Singleton<EffectPooler>
{
    protected override void Awake() => base.Awake();

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    [SerializeField] Pool[] pools;
    List<GameObject> spawnObjects;
    Dictionary<string, Queue<GameObject>> poolDictionary;


    public GameObject SpawnFromPool(string tag, Vector3 position = default, Quaternion rotation = default) =>
        _SpawnFromPool(tag, position, rotation);


    public List<GameObject> GetAllPools(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            throw new Exception($"Pool with tag {tag} doesn't exist.");

        return spawnObjects.FindAll(x => x.name == tag);
    }

  
    public T SpawnFromPool<T>(string tag, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = _SpawnFromPool(tag, position, rotation);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"Component not found");
        }
    }
    public List<T> GetAllPools<T>(string tag) where T : Component
    {
        List<GameObject> objects = GetAllPools(tag);

        if (!objects[0].TryGetComponent(out T component))
            throw new Exception("Component not found");

        return objects.ConvertAll(x => x.GetComponent<T>());
    }
    public void ReturnToPool(GameObject obj)
    {
        if (!poolDictionary.ContainsKey(obj.name))
            throw new Exception($"Pool with tag {obj.name} doesn't exist.");

        poolDictionary[obj.name].Enqueue(obj);
    }
    GameObject _SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
            throw new Exception($"Pool with tag {tag} doesn't exist.");


        Queue<GameObject> poolQueue = poolDictionary[tag];
        if (poolQueue.Count <= 0)
        {
            Pool pool = Array.Find(pools, x => x.tag == tag);
            var obj = CreateNewObject(pool.tag, pool.prefab);
            ArrangePool(obj);
        }

        GameObject objectToSpawn = poolQueue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }
    void Start()
    {
        spawnObjects = new List<GameObject>();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();


        foreach (Pool pool in pools)
        {
            poolDictionary.Add(pool.tag, new Queue<GameObject>());
            for (int i = 0; i < pool.size; i++)
            {
                var obj = CreateNewObject(pool.tag, pool.prefab);
                ArrangePool(obj);
            }

            if (poolDictionary[pool.tag].Count != pool.size)
                Debug.LogError($"{pool.tag} ReturnToPool");
        }
    }
    GameObject CreateNewObject(string tag, GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false);
        return obj;
    }
    void ArrangePool(GameObject obj)
    {
        bool isFind = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                spawnObjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
                isFind = true;
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                spawnObjects.Insert(i, obj);
                break;
            }
        }
    }



    [ContextMenu("GetSpawnObjectsInfo")]
    void GetSpawnObjectsInfo()
    {
        foreach (var pool in pools)
        {
            int count = spawnObjects.FindAll(x => x.name == pool.tag).Count;
            Debug.Log($"{pool.tag} count : {count}");
        }
    }
}