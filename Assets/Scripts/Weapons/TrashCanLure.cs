using UnityEngine;

// 垃圾桶诱饵：吸引并持续伤害周围的敌人
public class TrashCanLure : MonoBehaviour
{
    [Header("诱饵属性")]
    public float attractRadius = 8f; // 吸引范围
    public float pullForce = 3f;     // 吸扯怪物的力度
    public float lifeTime = 5f;      // 垃圾桶存在的时间
    
    [Header("嘲讽设置")]
    public float tauntRadius = 10f;  // 嘲讽范围
    
    [Header("爆炸伤害")]
    public float explosionRadius = 5f;    // 爆炸范围
    public float explosionDamageMultiplier = 1.5f; // 爆炸伤害倍率
    
    private float currentLifeTime;
    private float damage;
    private float damageInterval = 1f; // 每秒造成一次伤害
    private float damageTimer;
    private bool isDead = false; // 标记是否已死亡

    // 由武器生成它时调用，传入最终伤害值
    public void Initialize(float dmg)
    {
        damage = dmg;
        currentLifeTime = lifeTime;
        damageTimer = damageInterval;
    }

    void Update()
    {
        // 1. 寿命倒计时，时间到了就引爆
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0 && !isDead)
        {
            Explode();
            return;
        }

        // 2. 寻找范围内的所有怪物
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attractRadius);
        
        // 3. 处理伤害冷却计时
        bool shouldDamage = false;
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0)
        {
            shouldDamage = true;
            damageTimer = damageInterval; // 重置伤害计时器
        }

        // 4. 强行吸引怪物并造成伤害
        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                EnemyAI enemy = coll.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // 让敌人检查是否应该被嘲讽
                    enemy.CheckTaunt();
                    
                    // 将怪物的坐标强行向垃圾桶的方向拖拽（覆盖它的默认移动轨迹）
                    coll.transform.position = Vector2.MoveTowards(
                        coll.transform.position, 
                        transform.position, 
                        pullForce * Time.deltaTime
                    );

                    // 如果伤害冷却好了，对范围内的怪物扣血
                    if (shouldDamage)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
        }
    }
    
    
    // 在 Scene 窗口画出吸引范围，方便调整
    private void OnDrawGizmosSelected()
    {
        // 吸引范围
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
        
        // 嘲讽范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tauntRadius);
        
        // 爆炸范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    
    // 爆炸方法：对周围敌人造成范围伤害
    private void Explode()
    {
        isDead = true;
        
        // 计算爆炸伤害
        float explosionDamage = damage * explosionDamageMultiplier;
        
        // 查找爆炸范围内的所有敌人
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                EnemyAI enemy = coll.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }
        
        // TODO: 这里可以添加爆炸特效
        // GameObject explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // Destroy(explosionEffect, 2f);
        
        // 回收到对象池
        if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
        else gameObject.SetActive(false);
    }
}
