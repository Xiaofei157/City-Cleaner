using UnityEngine;

public enum CardRarity { White, Blue } // 白色卡，蓝色卡 
public enum StatType { HP, AttackSpeed, MoveSpeed, Damage }
public enum ValueType { Numeric, Percentage } // 数值提升或百分比提升 

[CreateAssetMenu(fileName = "NewUpgradeCard", menuName = "CityCleaner/Upgrade Card")]
public class UpgradeCard_SO : ScriptableObject
{
    [Header("卡片信息")]
    public string cardName; // 卡片上会显示的名称 
    public Sprite icon;     // 图片 
    [TextArea]
    public string description; // 功能描述 

    [Header("奖励规则")]
    public CardRarity rarity;     // 稀有度，影响不同等级的概率 
    public bool canAppearConsecutively; // 该奖励是否可以在同一关卡连续出现

    [Header("属性提升")]
    public StatType statType;     // 提升什么属性（血量/攻速等） 
    public ValueType valueType;   // 数值提升是指直接加，百分比是用公式算 
    public float value;           // 具体提升的点数 
}