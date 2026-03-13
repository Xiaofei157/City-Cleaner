using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    // 角色永久属性（通过成长树升级获得）
    public float maxHealth = 100f; // 最大生命值
    public float attackDamage = 10f; // 攻击力
    public float moveSpeed = 5f; // 移动速度
}

public class GameManager : MonoSingleton<GameManager>
{
    private const string SAVE_FILE_NAME = "game_data";

    // 标记是否已初始化完成
    public bool IsInitialized { get; private set; }
    public GameData GameData { get; set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // 在 Awake 中就加载数据，确保其他脚本访问时数据已就绪
        GameData = JsonManager.Instance.LoadData<GameData>(SAVE_FILE_NAME);

        // 标记为已初始化
        IsInitialized = true;

        // 详细日志：显示所有加载的数据
        Debug.Log("💾 GameManager 初始化成功！");
        Debug.Log(
            $"📊 加载的游戏数据 - 金币：{GameData.gold}, 最大生命：{GameData.maxHealth}, 攻击力：{GameData.attackDamage}, 移动速度：{GameData.moveSpeed}");
    }

    private void Start()
    {
        Debug.Log("🎮 GameManager 准备开始清理城市！");
        // 可选：如果需要可以在这里再次保存以确保数据同步
        SaveGame();
    }

    public void SaveGame()
    {
        JsonManager.Instance.SaveData(GameData, SAVE_FILE_NAME);
        Debug.Log("保存游戏数据...");
    }
}