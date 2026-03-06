using UnityEngine;

// 油污史莱姆专属AI：死后留下减速油污
public class GreaseSlimeAI : EnemyAI
{
    [Header("史莱姆专属特性")]
    public GameObject oilPuddlePrefab; // 油污的预制体

    // 重写死亡方法（注意：你需要去 EnemyAI.cs 里把 private void Die() 改成 protected virtual void Die() ）
    protected override void Die()
    {
        // 1. 生成油污留在原地
        if (PoolManager.Instance != null && oilPuddlePrefab != null)
        {
            GameObject puddle = PoolManager.Instance.Get(oilPuddlePrefab);
            puddle.transform.position = transform.position;
        }

        // 2. 执行父类的基础死亡逻辑（掉经验、掉金币、回收到池子）
        base.Die();
    }
}