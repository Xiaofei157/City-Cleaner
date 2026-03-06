using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    [Header("基础属性")]
    public float maxHealth = 100f; 
    public float currentHealth;
    public float moveSpeed = 5f; // 移动速度 

    [Header("战斗加成")]
    public float baseAttackBonus = 0f; // 基础攻击力提升 
    public float attackSpeedBonus = 0f; // 攻击速度百分比提升 
    
    [Header("升级系统")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10; // 升到下一级所需经验

    private void Start() 
    {
        currentHealth = maxHealth;
        // 游戏一开始，就把自身的血条和等级在 UI 上面初始化好
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIManager.Instance.UpdateLevel(currentLevel);
            UIManager.Instance.UpdateXP(currentXP, xpToNextLevel); // 新增：初始化经验显示
        }
    }

    // --- 伤害与血量系统 ---
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // 受伤后立刻更新 UI 血条
        if (UIManager.Instance != null) UIManager.Instance.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("清洁工倒下了！游戏结束！");
            // 稍后我们会在这里弹出包含获得金币和局外成长点数的结算界面 [cite: 8]
        }
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
        return weaponBaseDamage * (1f + baseAttackBonus / 100f);
    }

    public float GetAttackSpeedMultiplier() 
    {
        return 1f + attackSpeedBonus / 100f;
    }
}