using UnityEngine;

public struct PropertyData
{
    public float maxHealthDelta;        // 生命值变化量
    public float currentHealthDelta;    // 当前生命值变化量
    public float attackSpeedDelta;      // 攻速加成变化量
    public float moveSpeedDelta;        // 移动速度变化量
    public float moveSpeedPercent;      // 移动速度百分比变化
}

public class PlayerStats : MonoSingleton<PlayerStats> 
{
    //基础属性
    [field: SerializeField] public float BaseMaxHealth { get; private set; }  = 100f;   // 基础最大生命值
    [field: SerializeField] public float MaxHealth { get; private set; }              // 实际最大生命值（基础值 + GameData 中的加成）
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float BaseMoveSpeed { get; private set; } = 5f;     // 基础移动速度
    [field: SerializeField] public float MoveSpeed { get; private set; }               // 实际移动速度（基础值 + GameData 中的加成）
    
    //战斗加成
    public float BaseAttackBonus { get; private set; } = 0f; // 基础攻击力提升 
    public float AttackSpeedBonus { get; private set; } = 0f; // 攻击速度百分比提升 
    
    [Header("升级系统")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10; // 升到下一级所需经验

    // --- 金币系统 ---
    [Header("金币系统")]
    public int sessionGold = 0; // 当前局获得的金币（每局重置）
    
    /// <summary>
    /// 当前可用的总金币（包含之前积累的 + 本局获得的）
    /// </summary>
    public int CurrentGold 
    { 
        get
        {
            int savedGold = 0;
            if (GameManager.Instance != null && GameManager.Instance.GameData != null)
                savedGold = GameManager.Instance.GameData.gold;
            return savedGold + sessionGold;
        }
    }
    
    /// <summary>
    /// 玩家总共积累的金币（永久保存的部分）
    /// </summary>
    public int TotalGold 
    { 
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.GameData == null)
                return 0;
            return GameManager.Instance.GameData.gold;
        }
        set 
        { 
            if (GameManager.Instance != null && GameManager.Instance.GameData != null)
                GameManager.Instance.GameData.gold = value; 
        }
    }
    
   

    private void Start() 
    {
        // 设置当前血量为最大血量
     
        sessionGold = 0; // 每局游戏开始时重置本局金币
        
        MaxHealth = BaseMaxHealth + GameManager.Instance.GameData.maxHealth;
        MoveSpeed = BaseMoveSpeed + GameManager.Instance.GameData.moveSpeed;
        BaseAttackBonus = GameManager.Instance.GameData.attackDamage;
        CurrentHealth = MaxHealth;
        
        Debug.Log($"🎮 游戏开始！本局初始金币：{sessionGold}，总金币：{TotalGold}");
        Debug.Log($"📊 玩家属性 - 生命值：{CurrentHealth}/{MaxHealth}, 移动速度：{MoveSpeed}, 攻击力加成：{BaseAttackBonus}%");
        
        // 游戏一开始，就把自身的血条和等级在 UI 上面初始化好
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(CurrentHealth, MaxHealth);
            UIManager.Instance.UpdateLevel(currentLevel);
            UIManager.Instance.UpdateXP(currentXP, xpToNextLevel); // 新增：初始化经验显示
            UIManager.Instance.UpdateGold(sessionGold); // 游戏中显示本局获得的金币
        }
    }

    
    // --- 伤害与血量系统 ---
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        // 受伤后立刻更新 UI 血条
        if (UIManager.Instance != null) UIManager.Instance.UpdateHealth(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    private void Die()
    {
        Debug.Log("清洁工倒下了！游戏结束！");
        
        // 保存金币数据（将本局获得的金币累加到总金币）
        SaveGold();
        
        // 1 秒后显示结算面板
        Invoke(nameof(ShowGameOverPanel), 1f);
    }

    /// <summary>
    /// 显示结算面板
    /// </summary>
    private void ShowGameOverPanel()
    {
        // 停止时间流逝
        Time.timeScale = 0f;
        
        // 从 Resources 加载预制体
        GameObject panelPrefab = Resources.Load<GameObject>("jiesuPanel");
        if (panelPrefab != null)
        {
            // 实例化结算面板
            GameObject panelInstance = Instantiate(panelPrefab);
            
            // 确保添加到 Canvas 下
            Transform canvasTransform = FindCanvasTransform();
            if (canvasTransform != null)
            {
                panelInstance.transform.SetParent(canvasTransform, false);
            }
            
            // 添加 GameOverPanel 组件（如果预制体上没有）
            GameOverPanel gameOverPanel = panelInstance.GetComponent<GameOverPanel>();
            if (gameOverPanel == null)
            {
                gameOverPanel = panelInstance.AddComponent<GameOverPanel>();
            }
            
            Debug.Log("✅ 结算面板已显示");
        }
        else
        {
            Debug.LogError("❌ 未找到 jiesuPanel 预制体！请确保在 Resources 文件夹下");
        }
    }

    /// <summary>
    /// 查找 Canvas 的 Transform
    /// </summary>
    private Transform FindCanvasTransform()
    {
        // 先尝试在当前场景查找 Canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == "Canvas")
            {
                return canvas.transform;
            }
        }
        
        // 如果没有找到名为 Canvas 的，返回第一个找到的 Canvas
        if (canvases.Length > 0)
        {
            return canvases[0].transform;
        }
        
        Debug.LogWarning("⚠️ 未找到 Canvas，结算面板将没有父对象");
        return null;
    }

    // --- 经验与升级系统 ---
    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        else
        {
            // 新增：未升级时也更新经验 UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateXP(currentXP, xpToNextLevel);
            }
        }
    }
    
    // --- 金币系统 ---
    public void AddGold(int goldAmount)
    {
        sessionGold += goldAmount;
        Debug.Log($"💰 获得 {goldAmount} 金币！本局金币：{sessionGold}，总金币：{CurrentGold}");
        
        // 更新 UI 金币显示（显示本局获得的金币）
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGold(sessionGold);
        }
        
        // 保存金币数据（将本局金币累加到总金币）
        SaveGold();
    }

    /// <summary>
    /// 消耗金币（优先消耗永久金币）
    /// </summary>
    public bool SpendGold(int amount)
    {
        int availableGold = CurrentGold;
        if (availableGold >= amount)
        {
            // 优先从永久金币中扣除
            if (TotalGold >= amount)
            {
                TotalGold -= amount;
            }
            else
            {
                // 永久金币不够，扣除本局金币
                int remainingFromTotal = amount - TotalGold;
                TotalGold = 0;
                sessionGold -= remainingFromTotal;
            }
            
            Debug.Log($"💸 消耗 {amount} 金币！剩余总金币：{CurrentGold}（本局：{sessionGold}, 永久：{TotalGold}）");
            
            // 更新 UI 金币显示
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateGold(sessionGold);
            }
            
            // 保存金币数据
            SaveGold();
            return true;
        }
        else
        {
            Debug.LogWarning("⚠️ 金币不足！");
            return false;
        }
    }

    /// <summary>
    /// 保存金币数据（将本局金币累加到总金币并保存）
    /// </summary>
    private void SaveGold()
    {
        if (GameManager.Instance != null)
        {
            // 将本局获得的金币累加到总金币中
            if (sessionGold > 0)
            {
                TotalGold += sessionGold;
                sessionGold = 0; // 累加后重置本局金币
            }
            GameManager.Instance.SaveGame();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        
        if (UIManager.Instance != null) UIManager.Instance.UpdateLevel(currentLevel);
        
        // 新增：升级后更新经验 UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateXP(currentXP, xpToNextLevel);
        }

        // 【关键触发】弹出三选一界面
        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.ShowLevelUpUI();
        }
    }
    
    public float GetFinalDamage(float weaponBaseDamage) 
    {
        return weaponBaseDamage * (1f + BaseAttackBonus / 100f);
    }

    public float GetAttackSpeedMultiplier() 
    {
        return 1f + AttackSpeedBonus / 100f;
    }
    
    /// <summary>
    /// 改变玩家移速
    /// </summary>
    /// <summary>
    /// 应用属性修改（由外部传入修改数据）
    /// </summary>
    public void ApplyPropertyChange(PropertyData ctx)
    {
        // 应用生命值修改
        if (ctx.maxHealthDelta != 0)
        {
            MaxHealth += ctx.maxHealthDelta;
            CurrentHealth += ctx.currentHealthDelta;
            Debug.Log($"❤️ 生命值提升：{ctx.maxHealthDelta}，当前最大生命：{MaxHealth}");
        }
        
        // 应用攻速修改
        if (ctx.attackSpeedDelta != 0)
        {
            AttackSpeedBonus += ctx.attackSpeedDelta;
            Debug.Log($"⚡ 攻速提升：{ctx.attackSpeedDelta}%，当前攻速加成：{AttackSpeedBonus}%");
        }
        
        // 应用移速修改
        if (ctx.moveSpeedDelta != 0 || ctx.moveSpeedPercent != 0)
        {
            float delta = ctx.moveSpeedDelta + MoveSpeed * (ctx.moveSpeedPercent / 100f);
            MoveSpeed += delta;
            Debug.Log($"💨 移速提升：{delta}，当前移速：{MoveSpeed}");
        }
        
        // 更新 UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(CurrentHealth, MaxHealth);
        }
    }
    
    /// <summary>
    /// 改变角色移动速度
    /// </summary>
    /// <param name="multiplier"></param>
    public void ChangePlayerMoveSpeed(float multiplier)
    {
        MoveSpeed *= multiplier;
    }
}