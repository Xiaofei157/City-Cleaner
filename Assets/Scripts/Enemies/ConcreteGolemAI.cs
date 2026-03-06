using UnityEngine;

// 建筑垃圾怪 (精英)：高血量，周期性发动范围践踏攻击
public class ConcreteGolemAI : EnemyAI
{
    [Header("精英践踏技能")]
    public float stompRadius = 4f;    // 践踏伤害范围
    public float stompDamage = 20f;   // 践踏造成的巨大伤害
    public float stompCooldown = 5f;  // 每5秒践踏一次
    public float stompCastTime = 1f;  // 施法前摇时间（停下来蓄力）

    private float cooldownTimer;
    private bool isStomping = false;

    protected override void Update()
    {
        if (isStomping) return; // 蓄力践踏时不能移动

        base.Update(); // 正常追踪玩家

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            // 开始蓄力践踏
            isStomping = true;
            Invoke("PerformStomp", stompCastTime);
            
            // 提示：可以在这里改变怪物的颜色为红色，提示玩家快跑出范围！
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void PerformStomp()
    {
        // 恢复正常颜色
        GetComponent<SpriteRenderer>().color = Color.white;

        // 判定范围内的玩家并扣血
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, stompRadius);
        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Player"))
            {
                PlayerStats player = coll.GetComponent<PlayerStats>();
                if (player != null) player.TakeDamage(stompDamage);
            }
        }

        // 可以在这里播放屏幕震动或巨大声效
        // Debug.Log("建筑垃圾怪发动了战争践踏！");

        isStomping = false;
        cooldownTimer = stompCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stompRadius);
    }
}