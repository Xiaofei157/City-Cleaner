using UnityEngine;

// 管道污染怪：保持距离并吐毒液
public class PipeSludgeAI : EnemyAI
{
    [Header("远程攻击机制")]
    public float attackRange = 6f; // 走到离玩家多远时停下
    public float fireRate = 2.5f;  // 吐毒液的频率
    public GameObject sludgeBulletPrefab; // 拖入刚才做好的毒液预制体

    private float fireTimer;

    protected override void Update()
    {
        Transform target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer > attackRange)
        {
            // 距离太远，继续靠近（调用父类的走位逻辑）
            base.Update();
        }
        else
        {
            // 距离足够，不再移动，开始原地吐毒液！
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                FireSludge(target);
                fireTimer = fireRate;
            }
        }
    }

    private void FireSludge(Transform target)
    {
        if (PoolManager.Instance != null && sludgeBulletPrefab != null)
        {
            GameObject bullet = PoolManager.Instance.Get(sludgeBulletPrefab);
            bullet.transform.position = transform.position;
            
            Vector3 direction = target.position - transform.position;
            SludgeProjectile script = bullet.GetComponent<SludgeProjectile>();
            if (script != null) script.Initialize(direction);
        }
    }
}