using UnityEngine;

// 留在地上的油污，玩家踩上去会减速
public class OilPuddle : MonoBehaviour
{
    public float lifeTime = 5f; // 油污存在5秒
    private float currentLifeTime;

    void OnEnable()
    {
        currentLifeTime = lifeTime;
    }

    void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }

    // 当玩家踩入油污时减速
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null) player.moveSpeed *= 0.5f; // 减速50%
        }
    }

    // 玩家离开油污时恢复速度
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            // 注意：这里简单还原，实际项目中最好记录原始速度
            if (player != null) player.moveSpeed *= 2f; 
        }
    }
}