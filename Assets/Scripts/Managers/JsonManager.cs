using LitJson;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


/// <summary>
/// 序列化和反序列化Json时  使用的是哪种方案    有两种  JsonUtility 不能直接序列化字典  ligJson可以序列化字典 
/// </summary>
public enum JsonType
{
    JsonUtility,
    LitJson,
    Newtonsoft,
}
 
/// <summary>
/// Json数据管理类 主要用于进行 Json的序列化存储到硬盘 和 反序列化从硬盘中读取到内存中
/// </summary>
public class JsonManager : Singleton<JsonManager>
{
    public JsonManager()
    {
    }
 
    //存储 Json 数据 序列化
    public void SaveData(object data, string fileName, string directPath = "", JsonType type = JsonType.Newtonsoft)
    {
        //确定存储路径 - 使用宏区分编辑器模式和构建模式
        string directoryPath = "";
#if UNITY_EDITOR
        directoryPath = Application.dataPath + "/" + directPath;  // 编辑器模式：Assets 目录
#else
        directoryPath = Application.persistentDataPath + "/" + directPath;  // 构建模式：持久化目录
#endif
        string filePath = directoryPath + fileName + ".json";
        
        Debug.Log("存储路径：" + filePath);
        //序列化 得到Json字符串
        string jsonStr = "";
        switch (type)
        {
            case JsonType.JsonUtility:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
            case JsonType.Newtonsoft:
                jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                break;
        }
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        //把序列化的Json字符串 存储到指定路径的文件中
        File.WriteAllText(filePath, jsonStr);
    }
 
    //读取指定文件中的 Json 数据 反序列化
    public T LoadData<T>(string filePath, JsonType type = JsonType.Newtonsoft) where T : new()
    {
        //数据对象
        T data = new T();
        
        // 1. 编辑器模式下优先从 Assets 目录读取
#if UNITY_EDITOR
        string editorPath = Application.dataPath + "/" + filePath + ".json";
        if (File.Exists(editorPath))
        {
            string jsonStr = File.ReadAllText(editorPath);
            data = DeserializeJson<T>(jsonStr, type);
            return data;
        }
#endif
        
        // 2. 从 persistentDataPath (读写目录) 读取
        string persistentPath = Application.persistentDataPath + "/" + filePath + ".json";
        if (File.Exists(persistentPath))
        {
            string jsonStr = File.ReadAllText(persistentPath);
            data = DeserializeJson<T>(jsonStr, type);
            return data;
        }

        // 2. 如果读写目录没有，尝试从 StreamingAssets (只读目录) 读取默认配置
        // 注意：安卓平台 Application.streamingAssetsPath 是 jar:file:// 路径，不能使用 File.Exists / File.ReadAllText
        // 如果需要在安卓读取 StreamingAssets，建议使用 UnityWebRequest 或 BetterStreamingAssets
        // 这里为了兼容 PC 编辑器调试，保留 File 读取逻辑，但在安卓上会跳过（避免报错或无效路径）
        if (Application.platform != RuntimePlatform.Android)
        {
            string streamingPath = Application.streamingAssetsPath + "/" + filePath + ".json";
            if (File.Exists(streamingPath))
            {
                string jsonStr = File.ReadAllText(streamingPath);
                data = DeserializeJson<T>(jsonStr, type);
                return data;
            }
        }

        Debug.Log("没有找到文件，返回默认对象，路径：" + persistentPath);
        return data;
    }

    private T DeserializeJson<T>(string jsonStr, JsonType type)
    {
        T data = default(T);
        switch (type)
        {
            case JsonType.JsonUtility:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
            case JsonType.Newtonsoft:
                data = JsonConvert.DeserializeObject<T>(jsonStr);
                break;
        }
        return data;
    }
}