using UnityEngine;

// 清雪车召唤器：按频率召唤清雪车辅助
public class SnowplowWeapon : WeaponBase
{
    [Header("清雪车专属配置")]
    public GameObject snowplowPrefab; // 拖入清雪车实体的预制体
    public float spawnOffset = 1.5f;  // 生成时距离玩家多远（避免直接在头顶生成）
    public float attackRange = 10f;   // 索敌范围

    protected override void Attack()
    {
        if (PoolManager.Instance != null && snowplowPrefab != null && playerStats != null)
        {
            // 1. 寻找最近的敌人
            Transform nearestEnemy = GetNearestEnemy();
            if (nearestEnemy == null) return; // 没敌人的时候不召唤
                
            // 2. 计算朝向敌人的方向
            Vector2 directionToEnemy = (nearestEnemy.position - playerStats.transform.position).normalized;
            if (directionToEnemy == Vector2.zero) directionToEnemy = Vector2.right;
                
            // 3. 在玩家身边生成（稍微偏向敌人方向）
            Vector3 spawnPos = playerStats.transform.position + (Vector3)(directionToEnemy * spawnOffset);
            GameObject minion = PoolManager.Instance.Get(snowplowPrefab);
            minion.transform.position = spawnPos;
    
            // 4. 初始化清雪车的伤害和方向
            SnowplowMinion minionScript = minion.GetComponent<SnowplowMinion>();
            if (minionScript != null)
            {
                // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                minionScript.Initialize(finalDamage, directionToEnemy);
            }
        }
    }
        
    // 寻找最近的敌人
    private Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = playerStats.transform.position;
    
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