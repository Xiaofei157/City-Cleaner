using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected EnemyData_SO myData;
    protected float currentHP;
    protected Transform playerTarget;
    protected Transform currentTarget; // 当前实际攻击的目标（可能是玩家或垃圾桶）
    protected Animator animator; // 修改为 protected，允许子类访问
    
    [Header("攻击设置")]
    public float attackDamage = 5f; // 怪物对玩家的伤害
    public float attackInterval = 1f; // 多久造成一次伤害
    private float lastAttackTime = 0f;

    // 当从对象池中取出怪物时，调用此方法重置状态
    public void Initialize(EnemyData_SO data)
    {
        myData = data;
        currentHP = data.maxHP; // 从数据中读取血量
        
        // 获取 Animator 组件（如果敌人预制体上有）
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // 寻找玩家作为目标
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
        
        // 初始时当前目标就是玩家
        currentTarget = playerTarget;
        
        gameObject.SetActive(true); 
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("TrashCan"))
        {
            // 控制攻击频率，防止一瞬间扣光玩家血量
            if (Time.time - lastAttackTime >= attackInterval)
            {
                PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                TrashCanLure trashCan = collision.gameObject.GetComponent<TrashCanLure>();
                
                if (playerStats != null)
                {
                    playerStats.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
                else if (trashCan != null)
                {
                    // 攻击垃圾桶不需要造成伤害，只需要销毁垃圾桶
                    // 这里不做任何操作，让垃圾桶自己判断是否被攻击
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (currentTarget != null)
        {
            // 向当前目标方向移动
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, myData.moveSpeed * Time.deltaTime);
        }
    }

    // 在 EnemyAI 类中，你可以添加这个静态/全局方便读取的路径，或者直接用 Resources 加载
    // 这样不用去 8 个怪物的面板里挨个拖拽预制体！

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        
        // 【新增：生成伤害飘字】
        ShowDamageText(damage);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void ShowDamageText(float damage)
    {
        if (PoolManager.Instance != null)
        {
            // 直接从 Resources 文件夹加载预制体，方便省事
            GameObject textPrefab = Resources.Load<GameObject>("DamageText_Prefab");
            if (textPrefab != null)
            {
                GameObject textObj = PoolManager.Instance.Get(textPrefab);
                // 生成位置稍微在怪物头顶偏上一点
                textObj.transform.position = transform.position + Vector3.up * 0.5f;
                
                DamageText dmgText = textObj.GetComponent<DamageText>();
                if (dmgText != null)
                {
                    dmgText.Setup(damage);
                }
            }
        }
    }
    
    /// <summary>
    /// 检查附近的垃圾桶诱饵，如果被嘲讽则切换目标
    /// </summary>
    public void CheckTaunt()
    {
        // 查找附近所有的垃圾桶诱饵
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f); // 10f 是默认搜索半径
        
        TrashCanLure nearestTaunt = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("TrashCan")) // 确保垃圾桶预制体有 TrashCan 标签
            {
                TrashCanLure taunt = coll.GetComponent<TrashCanLure>();
                if (taunt != null)
                {
                    float distance = Vector2.Distance(transform.position, coll.transform.position);
                    
                    // 如果在嘲讽范围内且更近，则选择这个垃圾桶
                    if (distance <= taunt.tauntRadius && distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTaunt = taunt;
                    }
                }
            }
        }
        
        // 如果找到嘲讽的垃圾桶，切换目标；否则切换回玩家
        if (nearestTaunt != null)
        {
            currentTarget = nearestTaunt.transform;
        }
        else
        {
            currentTarget = playerTarget;
        }
    }
    
    /// <summary>
    /// 被垃圾钳夹取时播放特效
    /// </summary>
    public virtual void OnTongsClamped()
    {
        PlayClampEffect();
    }
    
    /// <summary>
    /// 在敌人身上播放垃圾钳夹取特效（不覆盖敌人本身）
    /// </summary>
    private void PlayClampEffect()
    {
        // 加载垃圾钳动画预制体
        GameObject tongsAnimPrefab = Resources.Load<GameObject>("WeaponAnim/Tongs_Animation");
        if (tongsAnimPrefab == null)
        {
            Debug.LogWarning("未找到垃圾钳动画预制体！请确保在 Resources/WeaponAnim/ 文件夹下创建了 Tongs_Animation 预制体");
            return;
        }
        
        // 在敌人位置实例化特效
        GameObject clampEffect = Instantiate(tongsAnimPrefab, transform.position, Quaternion.identity);
        
        // 设置为敌人的子对象（这样会跟随敌人移动）
        clampEffect.transform.SetParent(transform);
        
        // 重置本地位置，确保在敌人中心播放
        clampEffect.transform.localPosition = Vector3.zero;
        
        // 自动销毁特效（动画长度约 0.25 秒，给 0.5 秒保证播完）
        Destroy(clampEffect, 0.5f);
    }
    
    
    
    
    protected virtual void Die()
    {
        // 怪物死亡一定会掉落 xp
        DropXP(myData.xpDrop);
        // 放回对象池
        gameObject.SetActive(false);
    }

    private void DropXP(int amount) 
    { 
        // 从对象池拿一个经验球出来
        if (PoolManager.Instance != null)
        {
            // 注意：下一步我们需要去制作这个经验球预制体
            GameObject xpOrb = PoolManager.Instance.Get(Resources.Load<GameObject>("XPOrb"));
            if (xpOrb != null)
            {
                xpOrb.transform.position = transform.position;
                xpOrb.GetComponent<ExperienceOrb>().xpValue = amount;
            }
        }
    }
}