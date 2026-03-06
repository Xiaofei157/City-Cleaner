using UnityEngine;

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
    public int xpDrop;       // 死亡一定会掉落的xp值 
    [Range(0f, 1f)]
    public float goldDropChance; // 掉落金币的概率（不一定掉落） 

    [Header("生成规则")]
    public int minLevelAppearance; // 限定出现的最低关卡
    public int maxLevelAppearance; // 限定出现的最高关卡
}