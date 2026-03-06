using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public int xpValue = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 当碰到标签为 Player 的物体时
        if (collision.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.AddXP(xpValue);

                // 拾取后放回对象池（安全起见，如果没有对象池就直接隐藏）
                if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
                else gameObject.SetActive(false);
            }
        }
    }
}