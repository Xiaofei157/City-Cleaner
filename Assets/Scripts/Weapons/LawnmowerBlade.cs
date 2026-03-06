using UnityEngine;

// 除草机刀片陷阱：留在原地，怪物踩中后造成伤害并消失
public class LawnmowerBlade : MonoBehaviour
{
    public float lifeTime = 10f; // 刀片在地上最多存在10秒
    private float currentLifeTime;
    private float damage; // 最终伤害值

    // 由除草机生成它时调用，传入最终伤害
    public void Initialize(float dmg)
    {
        damage = dmg;
        currentLifeTime = lifeTime;
    }

    void Update()
    {
        // 寿命倒计时，时间到了没被踩也会自动回收到池子
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }

    // 当怪物踩到陷阱时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                // 可以加个除草机绞碎垃圾的音效
                // Debug.Log($"刀片陷阱切碎了 {collision.name}！");
            }
            
            // 陷阱是一次性的，触发后消失放回池子
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }
}