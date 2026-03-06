using UnityEngine;

// 清雪车辅助：沿直线狂飙，穿透并伤害沿途所有敌人
public class SnowplowMinion : MonoBehaviour
{
    public float speed = 8f;       // 清雪车行驶速度
    public float lifeTime = 4f;    // 存在时间，防止一直开到地图外
    
    private float currentLifeTime;
    private float damage;
    private Vector3 moveDirection;

    // 由武器召唤它时调用，传入伤害和行驶方向
    public void Initialize(float dmg, Vector3 direction)
    {
        damage = dmg;
        moveDirection = direction.normalized;
        currentLifeTime = lifeTime;

        // 让清雪车的车头（图片）对准行驶的方向
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // 持续向设定的方向移动
        transform.position += moveDirection * speed * Time.deltaTime;

        // 寿命倒计时，时间到了自动回收到池子
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }

    // 当碾压到怪物时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                // 造成碾压伤害（注意：不清空清雪车，它可以穿透群体敌人！）
                enemy.TakeDamage(damage);
            }
        }
    }
}