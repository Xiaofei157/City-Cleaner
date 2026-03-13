using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "CityCleaner/Enemy Data")]
public class EnemyData_SO : ScriptableObject
{
    [Header("基本信息")]
    public string enemyName; // 怪物名称 
    public GameObject prefab;// 预制体引用 

    [Header("战斗属性")]
    public float maxHP;      // 血量 
    public float moveSpeed;  // 移动速度 

    [Header("掉落设定")]
    public int xpDrop;       // 死亡一定会掉落的 xp 值 
    [Range(0f, 1f)]
    public float goldDropChance; // 掉落金币的概率（不一定掉落）
    public int goldDropMin = 1;   // 掉落金币的最小数量
    public int goldDropMax = 5;   // 掉落金币的最大数量（随机区间）

    [Header("生成规则")]
    public int minLevelAppearance; // 限定出现的最低关卡
    public int maxLevelAppearance; // 限定出现的最高关卡
    
    [Header("特殊怪物设置")]
    public bool isBoss = false;        // 是否为 BOSS
    public bool isElite = false;       // 是否为精英怪
    public List<int> specificLevels = new List<int>();   // 指定在第几关出现（为空则按 min/max Level 规则）
}