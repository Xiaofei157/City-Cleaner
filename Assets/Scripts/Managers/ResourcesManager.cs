using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 资源管理器
/// 负责管理和加载 Resources 文件夹中的各种资源
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }
    
    [Header("缓存的预制体")]
    private Dictionary<string, GameObject> cachedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 从缓存或 Resources 中加载预制体
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <returns>加载的预制体，如果不存在则返回 null</returns>
    public GameObject LoadPrefab(string prefabName)
    {
        // 先从缓存中查找
        if (cachedPrefabs.TryGetValue(prefabName, out GameObject cached))
        {
            return cached;
        }

        // 缓存中没有，尝试从 Resources 文件夹加载
        GameObject loaded = Resources.Load<GameObject>(prefabName);
        if (loaded != null)
        {
            // 加入缓存
            cachedPrefabs[prefabName] = loaded;
            Debug.Log($"📥 从 Resources 加载预制体：{prefabName}");
            return loaded;
        }
        else
        {
            Debug.LogError($"❌ 无法加载预制体：{prefabName}");
            return null;
        }
    }

    /// <summary>
    /// 实例化预制体
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    /// <param name="parent">父物体</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <returns>实例化的 GameObject</returns>
    public GameObject InstantiatePrefab(string prefabName, Transform parent = null, Vector3? position = null, Quaternion? rotation = null)
    {
        GameObject prefab = LoadPrefab(prefabName);
        if (prefab == null)
        {
            Debug.LogError($"❌ 无法实例化预制体：{prefabName}，预制体不存在");
            return null;
        }

        Vector3 pos = position ?? Vector3.zero;
        Quaternion rot = rotation ?? Quaternion.identity;

        GameObject instance = Instantiate(prefab, pos, rot, parent);
        Debug.Log($"✨ 实例化预制体：{prefabName}");
        return instance;
    }

    /// <summary>
    /// 从 Resources/Sprites 加载精灵
    /// </summary>
    /// <param name="spriteName">精灵名称</param>
    /// <returns>加载的精灵</returns>
    public Sprite LoadSprite(string spriteName)
    {
        return Resources.Load<Sprite>($"Sprites/{spriteName}");
    }

    /// <summary>
    /// 从 Resources/Audio 加载音频剪辑
    /// </summary>
    /// <param name="audioName">音频名称</param>
    /// <returns>加载的音频剪辑</returns>
    public AudioClip LoadAudioClip(string audioName)
    {
        return Resources.Load<AudioClip>($"Audio/{audioName}");
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public void ClearCache()
    {
        cachedPrefabs.Clear();
        Debug.Log("🗑️ 已清空所有预制体缓存");
    }

    /// <summary>
    /// 从缓存中移除指定预制体
    /// </summary>
    /// <param name="prefabName">预制体名称</param>
    public void RemoveFromCache(string prefabName)
    {
        if (cachedPrefabs.ContainsKey(prefabName))
        {
            cachedPrefabs.Remove(prefabName);
            Debug.Log($"🗑️ 已从缓存移除：{prefabName}");
        }
    }
}
