using UnityEngine;

// 废弃轮胎怪：周期性向玩家发起高速直线冲锋
public class TireTerrorAI : EnemyAI
{
    [Header("冲锋机制")]
    public float chargeMultiplier = 3f; // 冲锋时的速度倍率 (比如原本是2，冲锋就是6)
    public float chargeCooldown = 4f;   // 冲锋技能的冷却时间
    public float chargeDuration = 1f;   // 冲锋持续多久

    private float currentCooldown;
    private float currentChargeTime;
    private bool isCharging = false;
    private Vector3 chargeDirection;

    // 重写 Update 方法
    protected override void Update()
    {
        // 如果正在冲锋，跳过父类的 Update 逻辑
        if (isCharging)
        {
            // 冲锋状态：无视正常的追踪，沿锁定的方向高速直线移动
            currentChargeTime -= Time.deltaTime;
            
            // 注意：这里需要我们在 EnemyAI 里把 myData 改为 protected，或者你直接在这里写死一个速度测试
            transform.position += chargeDirection * (3f * chargeMultiplier) * Time.deltaTime;

            if (currentChargeTime <= 0)
            {
                StopCharge();
            }
            return;
        }
        
        // 非冲锋状态：正常执行父类 Update 逻辑
        base.Update();
        
        // 确保有目标才能继续
        if (currentTarget == null)
        {
            // 尝试重新获取玩家目标
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                currentTarget = player.transform;
            }
            else
            {
                return; // 没有目标，直接返回
            }
        }
        
        // 冷却计时
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0 && currentTarget != null)
        {
            StartCharge(currentTarget);
        }
    }

    private void StartCharge(Transform target)
    {
        isCharging = true;
        currentChargeTime = chargeDuration;
        // 锁定冲锋瞬间，玩家所在的方向
        chargeDirection = (target.position - transform.position).normalized;
        
        // 可以在这里改变轮胎的颜色或播放音效，提示玩家“它要撞过来了！”
    }

    private void StopCharge()
    {
        isCharging = false;
        currentCooldown = chargeCooldown;
    }
}