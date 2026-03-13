using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoSingleton<LevelUpManager>
{
    public GameObject levelUpPanel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    
    [Header("全部卡片池")]
    public List<UpgradeCard_SO> allNormalUpgrades;
    public List<WeaponData_SO> allPossibleWeapons;
    
    /// <summary>
    /// 显示升级 UI 并生成三张随机卡片
    /// </summary>
    public void ShowLevelUpUI()
    {
        // 0. 检查必要引用
        if (levelUpPanel == null)
        {
            Debug.LogError("LevelUpManager: levelUpPanel 为空！请在 Inspector 面板赋值。");
            return;
        }
        
        if (cardContainer == null)
        {
            Debug.LogError("LevelUpManager: cardContainer 为空！请在 Inspector 面板赋值。");
            return;
        }
        
        // 1. 暂停游戏
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);
    
        // 2. 清空旧卡片
        ClearAllCards();
    
        // 3. 生成新卡片
        GenerateThreeCards();
    }
    
    /// <summary>
    /// 清空容器中的所有卡片
    /// </summary>
    private void ClearAllCards()
    {
        if (cardContainer == null)
        {
            Debug.LogError("LevelUpManager: cardContainer 为空！无法清空卡片。");
            return;
        }
        
        // 先复制一份子对象列表，避免在遍历过程中修改集合导致的问题
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in cardContainer)
        {
            if (child != null)
            {
                childrenToDestroy.Add(child);
            }
        }
        
        // 统一销毁所有子对象
        foreach (Transform child in childrenToDestroy)
        {
            if (child != null)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// 生成三张升级卡片
    /// </summary>
    private void GenerateThreeCards()
    {
        PlayerWeaponManager pWeapon = FindObjectOfType<PlayerWeaponManager>();
            
        if (allNormalUpgrades == null || allNormalUpgrades.Count == 0)
        {
            Debug.LogError("LevelUpManager: allNormalUpgrades 列表为空，请在 Inspector 面板添加卡片数据！");
            return;
        }
    
        if (cardPrefab == null)
        {
            Debug.LogError("LevelUpManager: cardPrefab 没在 Inspector 面板赋值！");
            return;
        }
    
        for (int i = 0; i < 3; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            UpgradeCardUI ui = cardObj.GetComponent<UpgradeCardUI>();
                
            if (ui == null)
            {
                Debug.LogError("卡片预制体 prefab 上没有找到 UpgradeCardUI 脚本！请检查预制体。");
                Destroy(cardObj);
                continue;
            }
    
            InitializeCard(ui, i, pWeapon);
        }
    }
    
    /// <summary>
    /// 初始化单个卡片的内容
    /// </summary>
    private void InitializeCard(UpgradeCardUI ui, int cardIndex, PlayerWeaponManager pWeapon)
    {
        // 规则：如果武器不足 4 个且有可随机的武器，第一张卡优先出武器
        bool shouldTryWeapon = (cardIndex == 0 && pWeapon != null && pWeapon.equippedWeaponsData.Count < 4);
            
        if (shouldTryWeapon)
        {
            WeaponData_SO randomWeapon = GetRandomNewWeapon(pWeapon);
            if (randomWeapon != null)
            {
                ui.SetupWeapon(randomWeapon);
                return;
            }
        }
    
        // 出普通 Buff
        if (allNormalUpgrades != null && allNormalUpgrades.Count > 0)
        {
            UpgradeCard_SO randomBuff = allNormalUpgrades[Random.Range(0, allNormalUpgrades.Count)];
            ui.Setup(randomBuff);
        }
        else
        {
            Debug.LogWarning("LevelUpManager: 没有任何普通 Buff 可选！");
        }
    }

    private bool HasAvailableWeapons(PlayerWeaponManager pWeapon)
    {
        if (allPossibleWeapons == null) return false;
        return allPossibleWeapons.Exists(w => !pWeapon.equippedWeaponsData.Contains(w));
    }

    private WeaponData_SO GetRandomNewWeapon(PlayerWeaponManager pWeapon)
    {
        if (allPossibleWeapons == null || allPossibleWeapons.Count == 0) return null;
        
        // 逻辑：从所有武器池中排除掉已经拥有的
        List<WeaponData_SO> available = allPossibleWeapons.FindAll(w => w != null && !pWeapon.equippedWeaponsData.Contains(w));
        if (available.Count == 0) return null;
        return available[Random.Range(0, available.Count)];
    }

    // --- 应用效果 ---
    public void ApplyUpgrade(UpgradeCard_SO buff)
    {
        PlayerStats stats = PlayerStats.Instance;
            
        // 构建属性修改上下文
        PropertyData ctx = new PropertyData();
            
        // [cite_start]应用公式：数值提升直接加，百分比提升用原本的*(1+ 提升/100) [cite: 37, 38]
        switch (buff.statType)
        {
            case StatType.HP:
                ctx.maxHealthDelta = buff.value;
                ctx.currentHealthDelta = buff.value; // 加上限顺便加当前血量
                break;
            case StatType.AttackSpeed:
                ctx.attackSpeedDelta = buff.value;
                break;
            case StatType.MoveSpeed:
                ctx.moveSpeedPercent = buff.value; // 百分比提升
                break;
        }
            
        // 将修改数据传递给 PlayerStats 处理
        stats.ApplyPropertyChange(ctx);
            
        CloseUI();
    }

    public void ApplyNewWeapon(WeaponData_SO weapon)
    {
        FindObjectOfType<PlayerWeaponManager>().EquipWeapon(weapon);
        CloseUI();
    }

    private void CloseUI()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
    }
    
    // 从所有武器so中随机获取一个
    public WeaponData_SO GetRandomWeapon()
    {
        if (allPossibleWeapons == null || allPossibleWeapons.Count == 0) return null;
        return allPossibleWeapons[Random.Range(0, allPossibleWeapons.Count)];
    }
    
    // 从所有属性so中随机获取一个
    public UpgradeCard_SO GetRandomBuff()
    {
        if (allNormalUpgrades == null || allNormalUpgrades.Count == 0) return null;
        return allNormalUpgrades[Random.Range(0, allNormalUpgrades.Count)]; 
    }
}