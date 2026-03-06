using UnityEngine;

// 吸尘器：范围吸附与持续频率伤害
public class VacuumWeapon : WeaponBase
{
    [Header("吸尘器专属配置")]
    public float effectRadius = 4f; // 吸附与伤害的生效范围
    public float pullSpeed = 2f;    // 把怪物吸过来的速度

    protected override void Update()
    {
        // 必须调用基类的 Update，这样才能保证 Attack() 按频率触发！
        base.Update();

        if (playerStats == null) return;

        // 持续吸附逻辑：每一帧都在生效
        PullEnemies();
    }

    // 核心攻击：按设定的频率触发群体伤害
    protected override void Attack()
    {
        // 1. 获取玩家当前位置
        Vector2 center = transform.position;

        // 2. 找到范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, effectRadius);

        // 3. 遍历这些碰撞体，如果是怪物就扣血
        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                EnemyAI enemy = coll.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // 计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                    float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                    enemy.TakeDamage(finalDamage);
                    
                    // 可以取消下面的注释来测试伤害频率
                    // Debug.Log($"吸尘器对 {coll.name} 造成了 {finalDamage} 点范围伤害！");
                }
            }
        }
    }

    // 将范围内的敌人向玩家中心拉扯
    private void PullEnemies()
    {
        Vector2 center = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, effectRadius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                // 计算从怪物指向玩家的方向
                Vector2 directionToPlayer = (center - (Vector2)coll.transform.position).normalized;
                
                // 将怪物强行向玩家方向移动（吸附效果）
                coll.transform.position = Vector2.MoveTowards(
                    coll.transform.position, 
                    center, 
                    pullSpeed * Time.deltaTime
                );
            }
        }
    }

    // 这是一个 Unity 内置的回调，方便我们在编辑器里画出一个圈，直观看到吸尘器的范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f); // 半透明的青色
        Gizmos.DrawSphere(transform.position, effectRadius);
    }
}