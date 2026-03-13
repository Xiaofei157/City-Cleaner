using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStats stats;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // 用于控制角色朝向

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
            Vector2 targetVelocity = movementInput.normalized * stats.MoveSpeed;
            
            // 直接赋值给 velocity 属性
            rb.velocity = targetVelocity;
        }
        
        // 根据水平移动方向翻转角色朝向
        FlipSprite();
    }
    
    /// <summary>
    /// 根据移动方向翻转角色精灵的朝向
    /// </summary>
    private void FlipSprite()
    {
        // 只有当有水平移动时才翻转
        if (Mathf.Abs(movementInput.x) > 0.1f && spriteRenderer != null)
        {
            // 向右移动时 flipX 为 false，向左移动时 flipX 为 true
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }
}