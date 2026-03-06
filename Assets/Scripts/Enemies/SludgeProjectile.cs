using UnityEngine;

// 怪物发射的毒液子弹
public class SludgeProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 3f;
    
    private float currentLifeTime;
    private Vector3 moveDirection;
    private float damage = 10f; // 毒液对玩家的伤害

    public void Initialize(Vector3 dir)
    {
        moveDirection = dir.normalized;
        currentLifeTime = lifeTime;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
        
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }

    // 打中玩家扣血
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null) player.TakeDamage(damage);

            // 命中后子弹消失
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }
}