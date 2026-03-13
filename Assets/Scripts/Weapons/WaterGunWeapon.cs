using UnityEngine;

// 水枪：以玩家为中心向怪物方向发射水流 
public class WaterGunWeapon : WeaponBase
{
    [Header("水枪专属配置")]
    public GameObject waterBulletPrefab; // 水流子弹的预制体
    public float attackRange = 10f; // 索敌范围
    public float spawnOffset = 0.5f; // 子弹生成位置偏移（从玩家中心向外偏移）

    protected override void Attack()
    {
        // 1. 寻找最近的敌人方向并发射 
        Transform nearestEnemy = GetNearestEnemy();
        if (nearestEnemy == null) return; // 没敌人的时候不开火

        // 2. 从对象池获取子弹 
        if (PoolManager.Instance != null && waterBulletPrefab != null)
        {
            // 获取玩家位置作为发射点
            Vector3 playerPosition = transform.position;
            
            // 计算从玩家到敌人的方向
            Vector3 direction = (nearestEnemy.position - playerPosition).normalized;
            
            // 在玩家前方一定距离生成子弹，避免与玩家碰撞
            Vector3 spawnPosition = playerPosition + direction * spawnOffset;
            
            GameObject bullet = PoolManager.Instance.Get(waterBulletPrefab);
            bullet.transform.position = spawnPosition;

            // 3. 计算对准敌人的角度并旋转子弹
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 4. 初始化子弹的伤害
            WaterBullet bulletScript = bullet.GetComponent<WaterBullet>();
            if (bulletScript != null)
            {
                // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                bulletScript.Initialize(finalDamage);
            }
        }
    }

    // 寻找最近的敌人
    private Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy) continue; // 忽略对象池里休眠的怪物
            
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDistance && dist <= attackRange)
            {
                nearest = enemy.transform;
                minDistance = dist;
            }
        }
        return nearest;
    }
}