using UnityEngine;

// 扫把：周围旋转武器 
public class OrbitWeapon : WeaponBase 
{
    public float radius = 1.5f; // 扫把距离玩家的旋转半径
    public float rotationSpeed = 180f; // 旋转速度：一秒旋转多少度 
    private float currentAngle = 0f; // 记录当前的角度

    protected override void Attack() 
    {
        // 扫把的旋转是持续的，Attack 可以留空，或者用来播放挥舞音效
    }

    void LateUpdate() 
    {
        // 如果还没有被 Initialize 喂数据，就不执行
        if (playerStats == null) return;

        // 1. 累加角度
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // 2. 三角函数计算偏移量 (X = 半径 * Cos, Y = 半径 * Sin)
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;

        // 3. 强制设置世界坐标 = 玩家坐标 + 偏移量。完全免疫玩家转向的影响！
        transform.position = playerStats.transform.position + new Vector3(x, y, 0);
        
        // 4. 让武器自身的贴图也顺着圆周朝向外侧（可选）
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
    // 【新增内容】Unity 自带的触发器碰撞检测函数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到的东西，它的标签是 "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // 获取怪物身上的 EnemyAI 脚本
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            
            if (enemy != null && playerStats != null && weaponData != null)
            {
                // 严格按照文档公式计算最终伤害：(武器基础伤害) * (1 + 攻击力加成%)
                float finalDamage = playerStats.GetFinalDamage(weaponData.baseDamage);
                
                // 让怪物扣血！
                enemy.TakeDamage(finalDamage);
                
                // 可以在这里加一句 Debug 看看有没有生效
                Debug.Log($"扫把击中了 {collision.name}，造成了 {finalDamage} 点伤害！");
            }
        }
    }
}