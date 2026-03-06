using UnityEngine;

// 消毒喷雾：扇形范围持续伤害
public class SprayWeapon : WeaponBase
{
    [Header("消毒喷雾专属配置")]
    public float attackRadius = 6f; // 喷雾能喷多远
    public float sprayAngle = 60f;  // 扇形的角度范围（60度代表前方一个锥形区域）

    protected override void Update()
    {
        base.Update();
        
        // 让喷雾器的发射点始终跟随着清洁工
        if (playerStats != null) 
        {
            transform.position = playerStats.transform.position;
        }
    }

    protected override void Attack()
    {
        // 1. 寻找最近的垃圾污染物来确定喷洒方向
        Transform nearestEnemy = GetNearestEnemy();
        if (nearestEnemy == null) return; // 没敌人就不浪费消毒液

        Vector2 sprayDirection = (nearestEnemy.position - transform.position).normalized;

        // 让喷雾器（如果有图片的话）在视觉上转向目标敌人
        float angle = Mathf.Atan2(sprayDirection.y, sprayDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 2. 获取半径范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                // 3. 核心数学：计算怪物是否在我们的扇形角度内
                Vector2 directionToEnemy = (coll.transform.position - transform.position).normalized;
                
                // 如果喷雾方向和敌人方向的夹角 <= 扇形总角度的一半，说明被喷中了！
                if (Vector2.Angle(sprayDirection, directionToEnemy) <= sprayAngle / 2f)
                {
                    EnemyAI enemy = coll.GetComponent<EnemyAI>();
                    if (enemy != null)
                    {
                        // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                        float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                        enemy.TakeDamage(finalDamage);
                    }
                }
            }
        }
    }

    // 索敌逻辑：寻找攻击范围内的最近敌人
    private Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy) continue; 
            
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDistance && dist <= attackRadius)
            {
                nearest = enemy.transform;
                minDistance = dist;
            }
        }
        return nearest;
    }

    // 在 Scene 窗口画出辅助线，方便你调整喷雾的距离
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // 半透明绿色代表消毒液
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}