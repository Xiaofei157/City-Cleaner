using UnityEngine;

// 除草机：按照频率在玩家脚下生成刀片陷阱
public class LawnmowerWeapon : WeaponBase
{
    [Header("除草机专属配置")]
    public GameObject bladePrefab; // 拖入刀片陷阱的预制体

    protected override void Attack()
    {
        // 确保对象池和预制体都存在
        if (PoolManager.Instance != null && bladePrefab != null && playerStats != null)
        {
            // 1. 从对象池获取一个刀片
            GameObject blade = PoolManager.Instance.Get(bladePrefab);
            
            // 2. 将其位置设置为玩家当前的脚下位置
            blade.transform.position = playerStats.transform.position;

            // 3. 初始化刀片的伤害
            LawnmowerBlade bladeScript = blade.GetComponent<LawnmowerBlade>();
            if (bladeScript != null)
            {
                // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                bladeScript.Initialize(finalDamage);
            }
        }
    }
}