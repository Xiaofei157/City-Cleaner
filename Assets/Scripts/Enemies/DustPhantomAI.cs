using UnityEngine;

// 灰尘幽灵：周期性进入无敌的幽灵状态
public class DustPhantomAI : EnemyAI
{
    [Header("幽灵专属特性")]
    public float solidDuration = 3f;  // 实体状态持续时间（可被攻击）
    public float ghostDuration = 2f;  // 幽灵状态持续时间（无敌）
    
    private bool isGhost = false;
    private float stateTimer;
    private SpriteRenderer spriteRenderer;

    // 当怪物从对象池生成时，获取它的图片组件并重置状态
    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isGhost = false;
        stateTimer = solidDuration;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    // 在 Update 里加上周期性切换状态的逻辑
    protected override void Update()
    {
        base.Update(); // 继续追踪玩家

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            // 切换状态
            isGhost = !isGhost;
            stateTimer = isGhost ? ghostDuration : solidDuration;

            // 视觉反馈：变成幽灵时图片半透明
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = isGhost ? 0.3f : 1f; // 0.3是半透明，1是完全不透明
                spriteRenderer.color = c;
            }
        }
    }

    // 重写受伤逻辑
    public override void TakeDamage(float damage)
    {
        // 如果当前是幽灵状态，直接免疫伤害！
        if (isGhost)
        {
            // 可以加个“MISS”的飘字
            return; 
        }

        // 如果是实体状态，正常扣血
        base.TakeDamage(damage);
    }
}