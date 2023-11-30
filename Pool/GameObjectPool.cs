using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameObjectPool
{
    private static Dictionary<string, Stack<GameObjectPoolItem>> Pool = new();
    private static Transform PoolParent;

    public static void Populate(GameObjectPoolItem poolItem, int count)
    {
        var id = poolItem.ID;
        var include = Pool.ContainsKey(id);
        
        if (include) return;

        if (PoolParent == null)
        {
            PoolParent = new GameObject().transform;
            PoolParent.name = "*Game Object Pool Populated Items*";
            
            Object.DontDestroyOnLoad(PoolParent);
        }

        var poolItemParent = new GameObject().transform;
        poolItemParent.SetParent(PoolParent);
        poolItemParent.name = $"Populated Items: {poolItem.ID}";
        //poolItemParent.hierarchyCapacity = count;
        
        Pool[id] = new Stack<GameObjectPoolItem>();
        for (int i = 0; i < count; i++)
        {
            InstantiatePoolObject(poolItem, poolItemParent);
        }
    }

    public static GameObjectPoolItem Spawn(GameObjectPoolItem poolItem, bool emptyParent = true)
    {
        var id = poolItem.ID;
        var include = Pool.ContainsKey(id);

        if (!include) Populate(poolItem, 1);

        var result = Pool[id].TryPop(out var popedItem);
        if (!result) return InstantiatePoolObject(poolItem, populate: false);
        
        if (emptyParent) popedItem.transform.parent = null;
        popedItem.gameObject.SetActive(true);

        return popedItem;
    }

    public static GameObjectPoolItem Spawn(GameObjectPoolItem poolItem, Transform parent)
    {
        var pooledGameObject = Spawn(poolItem, parent.position, parent.rotation, emptyParent: false);

        if (!pooledGameObject) return null;

        ChangePoolObjectScene(pooledGameObject, parent.gameObject.scene);
        pooledGameObject.transform.SetParent(parent);
        
        return pooledGameObject;
    }
    
    public static GameObjectPoolItem Spawn(GameObjectPoolItem poolItem, Vector3 position, Quaternion rotation, bool emptyParent = true)
    {
        var pooledGameObject = Spawn(poolItem, emptyParent);

        if (!pooledGameObject) return null;

        pooledGameObject.transform.position = position;
        pooledGameObject.transform.rotation = rotation;
        
        return pooledGameObject;
    }
    
    public static T Spawn<T>(GameObjectPoolItem poolItem, Vector3 position, Quaternion rotation)
    {
        var pooledGameObject = Spawn(poolItem, position, rotation);

        if (!pooledGameObject) return default;

        return pooledGameObject.GetComponent<T>();
    }    
    
    public static T Spawn<T>(GameObjectPoolItem poolItem, Transform parent)
    {
        var pooledGameObject = Spawn(poolItem, parent);

        if (!pooledGameObject) return default;

        return pooledGameObject.GetComponent<T>();
    }    
    
    public static T Spawn<T>(GameObjectPoolItem poolItem)
    {
        var pooledGameObject = Spawn(poolItem);

        if (!pooledGameObject) return default;

        return pooledGameObject.GetComponent<T>();
    }

    public static void Despawn(GameObjectPoolItem poolItem)
    {
        var id = poolItem.ID;
        var include = Pool.ContainsKey(id);

        if (!include) return;
        
        Pool[id].Push(poolItem);
        
        ChangePoolObjectScene(poolItem, poolItem.PoolParent.gameObject.scene);

        poolItem.gameObject.SetActive(false);
        poolItem.transform.SetParent(poolItem.PoolParent);
    }

    private static void ChangePoolObjectScene(GameObjectPoolItem poolItem, Scene scene)
    {
        var isSameScene = poolItem.gameObject.scene != scene;
        if (isSameScene) return;
        
        poolItem.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(poolItem.gameObject, scene);
    }

    public static void Despawn(GameObject gameObject)
    {
        Despawn(gameObject.GetComponent<GameObjectPoolItem>());
    }

    private static GameObjectPoolItem InstantiatePoolObject(GameObjectPoolItem poolItem, Transform parent = null, bool populate = true)
    {
        poolItem.PoolParent = parent == null ? PoolParent.Find($"Populated Items: {poolItem.ID}") : parent;
        
        var spawnedGameObjectPoolItem = parent == null
            ? Object.Instantiate(poolItem.gameObject).GetComponent<GameObjectPoolItem>()
            : Object.Instantiate(poolItem.gameObject, parent, true).GetComponent<GameObjectPoolItem>();
        
        if (!populate) return spawnedGameObjectPoolItem;

        Pool[poolItem.ID].Push(spawnedGameObjectPoolItem);
        spawnedGameObjectPoolItem.gameObject.SetActive(false);
        
        return spawnedGameObjectPoolItem;
    }
}