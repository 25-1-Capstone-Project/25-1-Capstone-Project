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
    public class PoolGroup
    {
        public string groupName;
        public Pool[] pools;
    }

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private PoolGroup[] groupedPools;
  
    private List<GameObject> spawnObjects;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Pool> poolInfoLookup;

    void Start()
    {
        spawnObjects = new List<GameObject>();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolInfoLookup = new Dictionary<string, Pool>();

        foreach (var group in groupedPools)
        {
            foreach (var pool in group.pools)
            {
                if (poolDictionary.ContainsKey(pool.tag))
                {
                    Debug.LogWarning($"Duplicate pool tag: {pool.tag}. Skipping.");
                    continue;
                }

                poolDictionary.Add(pool.tag, new Queue<GameObject>());
                poolInfoLookup.Add(pool.tag, pool);

                for (int i = 0; i < pool.size; i++)
                {
                    var obj = CreateNewObject(pool.tag, pool.prefab);
                    ArrangePool(obj);
                }

                if (poolDictionary[pool.tag].Count != pool.size)
                    Debug.LogError($"{pool.tag} ReturnToPool mismatch");
            }
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position = default, Quaternion rotation = default) =>
        _SpawnFromPool(tag, position, rotation);

    public T SpawnFromPool<T>(string tag, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = _SpawnFromPool(tag, position, rotation);
        if (obj.TryGetComponent(out T component))
            return component;

        obj.SetActive(false);
        throw new Exception($"Component of type {typeof(T)} not found on object with tag {tag}");
    }

    public List<GameObject> GetAllPools(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            throw new Exception($"Pool with tag {tag} doesn't exist.");

        return spawnObjects.FindAll(x => x.name == tag);
    }

    public List<T> GetAllPools<T>(string tag) where T : Component
    {
        var objects = GetAllPools(tag);

        if (objects.Count == 0 || !objects[0].TryGetComponent(out T _))
            throw new Exception($"Component of type {typeof(T)} not found in pool with tag {tag}");

        return objects.ConvertAll(x => x.GetComponent<T>());
    }

    public void ReturnToPool(GameObject obj)
    {
        if (!poolDictionary.ContainsKey(obj.name))
            throw new Exception($"Pool with tag {obj.name} doesn't exist.");

        poolDictionary[obj.name].Enqueue(obj);
    }

    private GameObject _SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
            throw new Exception($"Pool with tag {tag} doesn't exist.");

        Queue<GameObject> poolQueue = poolDictionary[tag];
        if (poolQueue.Count <= 0)
        {
            if (!poolInfoLookup.TryGetValue(tag, out var pool))
                throw new Exception($"No pool info found for tag: {tag}");

            var obj = CreateNewObject(pool.tag, pool.prefab);
            ArrangePool(obj);
        }

        GameObject objectToSpawn = poolQueue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    private GameObject CreateNewObject(string tag, GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false);
        return obj;
    }

    private void ArrangePool(GameObject obj)
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

        poolDictionary[obj.name].Enqueue(obj);
    }

    [ContextMenu("GetSpawnObjectsInfo")]
    void GetSpawnObjectsInfo()
    {
        foreach (var group in groupedPools)
        {
            foreach (var pool in group.pools)
            {
                int count = spawnObjects.FindAll(x => x.name == pool.tag).Count;
                Debug.Log($"{pool.tag} count : {count}");
            }
        }
    }
}