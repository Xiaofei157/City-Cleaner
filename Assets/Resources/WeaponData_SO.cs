using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "CityCleaner/Weapon Data")]
public class WeaponData_SO : ScriptableObject
{
    [Header("基本信息")]
    public string weaponName; // 武器名称 
    public Sprite icon;       // 武器图片，方便UI里面显示内容 
    public string description;// 武器描述 

    [Header("战斗属性")]
    public GameObject prefab; // 对应的武器预制体 
    public float baseDamage;  // 基础伤害 
    public float baseFireRate;// 攻击速度（统一为1秒多少次） 
}