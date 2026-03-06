using UnityEngine;

// 垃圾钳：锁定目标并在其位置进行夹取范围攻击
public class TongsWeapon : WeaponBase
{
    [Header("垃圾钳专属配置")]
    public float attackRange = 5f; // 寻找敌人的最大范围（手柄有多长）
    public float clampRadius = 1.5f; // 夹取造成的范围伤害半径（钳口有多大）

    protected override void Attack()
    {
        // 1. 寻找攻击范围内的最近敌人作为夹取目标
        Transform target = GetNearestEnemy();
        if (target == null) return; // 没目标就不空夹

        // 2. 在目标位置触发“夹取”范围伤害
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(target.position, clampRadius);

        bool hasClamped = false;

        // 3. 遍历被夹到的怪物，造成伤害并播放特殊动画
        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                    float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                    enemy.OnTongsClamped();
                    enemy.TakeDamage(finalDamage);
                    
                    // 新增：如果被垃圾钳击中，播放特殊的被夹取动画
                    
                    hasClamped = true;
                }
            }
        }

        // 4. (可选) 播放夹取的特效或音效
        if (hasClamped)
        {
             Debug.Log($"垃圾钳在 {target.name} 的位置发动了夹取攻击，粉碎了周围的垃圾！");
            // 后续我们可以在这里实例化一个钳子合拢的动画特效
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

    // 在编辑器里画出索敌范围，方便策划调数值
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 黄圈：索敌范围
    }
}