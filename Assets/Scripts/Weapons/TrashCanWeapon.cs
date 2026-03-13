using UnityEngine;

// 垃圾桶武器：定时在玩家周围生成垃圾桶诱饵
public class TrashCanWeapon : WeaponBase
{
    [Header("垃圾桶专属配置")]
    public GameObject trashCanPrefab; // 拖入垃圾桶诱饵的预制体
    public float spawnRadius = 3f;    // 在玩家周围多大范围内随机生成

    protected override void Attack()
    {
        if (PoolManager.Instance != null && trashCanPrefab != null)
        {
            // 1. 在玩家附近计算一个随机坐标
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = playerStats.transform.position + (Vector3)randomOffset;

            // 2. 从对象池中拿出一个垃圾桶
            GameObject trashCan = PoolManager.Instance.Get(trashCanPrefab);
            trashCan.transform.position = spawnPos;

            // 3. 初始化垃圾桶的伤害
            TrashCanLure lure = trashCan.GetComponent<TrashCanLure>();
            if (lure != null)
            {
                // 文档中垃圾桶基础伤害为空，我们这里取默认值 10
                float baseDmg = weaponData.baseDamage > 0 ? weaponData.baseDamage : 5f;
                // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                float finalDamage = playerStats.GetFinalDamage(baseDmg);
                
                lure.Initialize(finalDamage);
            }
        }
    }
}