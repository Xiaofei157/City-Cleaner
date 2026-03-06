using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStats stats;
    private Vector2 movementInput;
    private Rigidbody2D rb;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        
        // 确保没有重力影响
        if (rb != null) rb.gravityScale = 0f; 
    }

    void Update()
    {
        // 获取 WASD 或 方向键的输入 (-1 到 1)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 直接设置速度实现移动，响应更即时
        if (rb != null && stats != null)
        {
            // 计算目标速度：移动方向 * 速度
            Vector2 targetVelocity = movementInput.normalized * stats.moveSpeed;
            
            // 直接赋值给 velocity 属性
            rb.velocity = targetVelocity;
        }
    }
}