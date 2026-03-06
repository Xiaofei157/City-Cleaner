using UnityEngine;

/// <summary>
/// 精英敌人示例 - 演示如何重写动画事件实现不同的动画效果
/// </summary>
public class EliteEnemy : EnemyAI
{
    // 可以添加精英特有的属性
    [Header("精英特有配置")]
    public GameObject deathEffectPrefab; // 死亡特效
    

    // 重写被夹取动画 - 播放更夸张的特效
    public override void OnTongsClamped()
    {
        UnityEngine.Debug.Log("精怪物被垃圾钳夹住了！");
        
        // 调用基类方法播放垃圾钳特效
        base.OnTongsClamped();
        
        // 可以在这里添加精英特有的逻辑，比如发光、音效等
        Debug.Log("怪物受到夹击，非常愤怒！");
    }
    
    // 重写死亡动画 - 播放特殊的死亡动画和特效
    protected override void Die()
    {
        // 生成死亡特效
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        Debug.Log("精英怪物被消灭了！掉落更多奖励！");
        
        // 调用基类的死亡逻辑（掉落 XP 等）
        base.Die();
    }
}
