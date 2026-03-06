using UnityEngine;

// 除胶铲刀：紧贴玩家近身高速旋转，类似电锯光环
public class ScraperWeapon : WeaponBase
{
    [Header("除胶铲刀配置")]
    public float rotationSpeed = 720f; // 视觉上的旋转速度 (度/秒)
    public float attackRadius = 1.2f;  // 近身切割的伤害范围

    protected override void Update()
    {
        // 必须调用基类的 Update，让它计算冷却时间并触发 Attack()
        base.Update();

        if (playerStats == null) return;

        // 1. 始终紧贴玩家中心 (近身)
        transform.position = playerStats.transform.position;

        // 2. 自身视觉上高速旋转
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    // 核心攻击：每当冷却结束，对近身范围内的敌人造成一次切割伤害
    protected override void Attack()
    {
        // 获取近身范围内的所有碰撞体
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        
        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                    float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                    enemy.TakeDamage(finalDamage);
                }
            }
        }
    }

    // 在编辑器里画出红圈，方便你直观调整“近身”的范围有多大
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}