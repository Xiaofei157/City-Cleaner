using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    // 从对象池获取物体
    public GameObject Get(GameObject prefab)
    {
        string key = prefab.name;

        // 如果池子不存在，先建个空池子
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary.Add(key, new Queue<GameObject>());
        }

        // 尝试从池子里拿
        if (poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // 池子里没有，才进行实例化，并修改名字去掉"(Clone)"后缀方便管理
        GameObject newObj = Instantiate(prefab);
        newObj.name = prefab.name;
        return newObj;
    }

    // 将物体放回对象池 (怪物死亡或子弹消失时调用)
    public void Return(GameObject obj)
    {
        obj.SetActive(false); // 不 Destroy，而是 SetActive(false) 放回池子 
        if (poolDictionary.ContainsKey(obj.name))
        {
            poolDictionary[obj.name].Enqueue(obj);
        }
    }
    
    /// <summary>
    /// 清空所有对象池（场景切换时调用，防止引用已销毁的对象）
    /// </summary>
    public void ClearAllPools()
    {
        poolDictionary.Clear();
    }
}