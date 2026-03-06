using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    public float speed = 10f; // 水流飞行速度
    public float lifeTime = 2f; // 存活时间，防止飞出屏幕外永远不消失
    private float currentLifeTime;
    private float damage; // 最终伤害值

    // 由水枪开火时调用，传入计算好的最终伤害
    public void Initialize(float damageAmount)
    {
        damage = damageAmount;
        currentLifeTime = lifeTime;
    }

    void Update()
    {
        // 持续向物体自身的正前方（右侧）移动
        // 由于子弹已经被旋转到朝向敌人，所以使用 transform.right
        transform.Translate(transform.right * speed * Time.deltaTime);
        
        // 寿命倒计时，时间到了回收进对象池 
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // 命中怪物后，水流消失回收到池子 
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }
}