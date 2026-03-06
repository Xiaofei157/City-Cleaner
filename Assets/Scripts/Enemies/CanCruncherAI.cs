using UnityEngine;

// 废弃罐头怪：拥有极高防御力，每次受击最多只掉固定的微小血量
public class CanCruncherAI : EnemyAI
{
    [Header("罐头怪专属特性")]
    public float maxDamageTakenPerHit = 2f; // 无论玩家伤害多高，单次最多只受2点伤害

    // 重写受伤逻辑
    public override void TakeDamage(float damage)
    {
        // 机制：强制削弱玩家的单发爆发伤害，考验攻击频率
        float actualDamage = Mathf.Min(damage, maxDamageTakenPerHit);
        
        // 调用父类原本的扣血逻辑，但传入的是被大幅度削减后的真实伤害
        base.TakeDamage(actualDamage);
        
        // 可选：在这里播放“铛铛铛”的金属弹刀音效，反馈给玩家“它很硬”
    }
}