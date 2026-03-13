using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     金币解锁按钮组件
///     用于 Add Page 中需要金币解锁的按钮
/// </summary>
public class UpgradeTreeNode : MonoBehaviour
{
    [Header("前驱节点")] [SerializeField] private List<UpgradeTreeNode> prevNodes = new();

    [Header("后继节点")] [SerializeField] private List<UpgradeTreeNode> nextNodes = new();

    [Header("金币设置")] [Tooltip("解锁此按钮需要的金币数量")]
    public int requiredGold = 100; // 可在 Inspector 中配置

    [Header("升级类型")]
    [Tooltip("点击按钮后增加的属性类型")]
    public UpgradeType upgradeType = UpgradeType.None; // 升级类型
    
    [Tooltip("每次升级增加的数值")]
    public float upgradeValue = 0f; // 升级数值

    // 升级类型枚举
    public enum UpgradeType
    {
        None,           // 无效果（仅解锁）
        MaxHealth,      // 增加最大生命值
        AttackDamage,   // 增加攻击力
        MoveSpeed       // 增加移动速度
    }

    [Header("引用（自动获取）")] private Button button;

    private Image buttonImage;

    private Material buttonMaterial;

    // 当前节点状态
    public bool isLocked { get; private set; } = true;

    // 是否可解锁
    public bool canBeUnlocked { get; private set; }

    private void Awake()
    {
        // 获取组件
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClicked);

        // 初始化状态为锁定（透明度 1，完全显示）
        SetLockedState(true);

        InitializeNodes();
        
        // 初始化材质发光效果
        InitializeGlowEffect();
    }

    private void InitializeNodes()
    {
        // 如果没有前驱节点，则可以直接解锁
        if (prevNodes == null || prevNodes.Count == 0)
            canBeUnlocked = true;
        else
            // 检查所有前驱节点是否都已解锁
            UpdateUnlockStatus();
    }

    /// <summary>
    ///     初始化发光效果
    /// </summary>
    private void InitializeGlowEffect()
    {
        if (buttonImage == null) return;
        
        // 为每个按钮分配新材质实例，避免共享材质
        buttonMaterial = new Material(buttonImage.material);
        buttonImage.material = buttonMaterial;
        
        // 检查自身状态然后决定开启发光还是关闭
        if (isLocked && canBeUnlocked)
        {
            // 锁定且可以解锁：开启发光
            EnableGlow(true);
        }
        else
        {
            // 其他情况：关闭发光
            EnableGlow(false);
        }
    }

    /// <summary>
    ///     更新当前节点的可解锁状态
    /// </summary>
    public void UpdateUnlockStatus()
    {
        bool oldCanBeUnlocked = canBeUnlocked;
        
        // 如果没有前驱节点，可以直接解锁
        if (prevNodes == null || prevNodes.Count == 0)
        {
            canBeUnlocked = true;
        }
        else
        {
            // 检查所有前驱节点是否都已解锁（isLocked 为 false）
            var allPrevUnlocked = true;
            foreach (var prevNode in prevNodes)
                if (prevNode != null && prevNode.isLocked)
                {
                    allPrevUnlocked = false;
                    break;
                }

            canBeUnlocked = allPrevUnlocked;
        }
        
        // 只有当状态改变时才更新发光
        if (canBeUnlocked != oldCanBeUnlocked && isLocked)
        {
            UpdateGlowEffect();
        }
    }

    /// <summary>
    ///     通知后继节点更新状态并开启发光
    /// </summary>
    private void NotifySuccessors()
    {
        if (nextNodes != null)
            foreach (var nextNode in nextNodes)
                if (nextNode != null)
                {
                    // 后继节点更新自己的状态和发光
                    nextNode.UpdateUnlockStatus();
                }
    }

    /// <summary>
    ///     更新发光效果状态
    /// </summary>
    private void UpdateGlowEffect()
    {
        if (buttonMaterial == null) return;
        
        // 只有当节点被锁定且可以被解锁时才开启发光
        if (isLocked && canBeUnlocked)
        {
            EnableGlow(true);
        }
        else
        {
            EnableGlow(false);
        }
    }

    /// <summary>
    ///     启用或禁用发光效果
    /// </summary>
    private void EnableGlow(bool enable)
    {
        if (buttonMaterial == null) return;
        
        // 通过设置 shader 的 _GLOW_ON 关键字来控制发光
        if (enable)
        {
            buttonMaterial.EnableKeyword("_GLOW_ON");
            Debug.Log($"✅ {gameObject.name} 开启发光");
        }
        else
        {
            buttonMaterial.DisableKeyword("_GLOW_ON");
            Debug.Log($"❌ {gameObject.name} 关闭发光");
        }
    }

    /// <summary>
    ///     解锁按钮
    /// </summary>
    public void Unlock()
    {
        // 重新检查前驱节点状态，确保数据是最新的
        UpdateUnlockStatus();
        
        // 检查是否可以解锁
        if (!canBeUnlocked)
        {
            Debug.LogWarning("⚠️ 前驱节点未解锁，无法解锁此节点！");
            return;
        }

        // 检查 GameManager 和 GameData 是否存在
        if (GameManager.Instance == null || GameManager.Instance.GameData == null)
        {
            Debug.LogError("❌ GameManager 或 GameData 为空，无法解锁！");
            return;
        }

        // 扣除金币
        if (GameManager.Instance.GameData.gold >= requiredGold)
        {
            GameManager.Instance.GameData.gold -= requiredGold;
            isLocked = false;
            SetLockedState(false);
            Debug.Log($"✅ 解锁成功！消耗 {requiredGold} 金币");

            // 应用属性升级
            ApplyUpgrade();

            // 刷新金币显示
            RefreshGoldDisplay();
            
            // 关闭自己的发光效果
            EnableGlow(false);
            
            // 通知后继节点更新状态并开启发光
            NotifySuccessors();
        }
        else
        {
            Debug.LogWarning("⚠️ 金币不足，无法解锁！");
        }
    }

    /// <summary>
    ///     设置锁定/解锁状态
    /// </summary>
    private void SetLockedState(bool locked)
    {
        if (buttonImage == null) return;
        
        var color = buttonImage.color;
        
        if (locked)
        {
            // 锁定状态：Alpha 为 0.7（半透明显示）
            color.a = 0.5f;
        }
        else
        {
            // 解锁状态：Alpha 为 0（完全透明）
            color.a = 0f;
            
            // 可选：禁用按钮交互
            if (button != null) button.interactable = false;
        }
        
        buttonImage.color = color;
    }

    /// <summary>
    ///     刷新金币显示
    /// </summary>
    private void RefreshGoldDisplay()
    {
        // 查找场景中的 StartPageUI 并刷新金币显示
        var startPageUI = FindObjectOfType<StartPageUI>();
        if (startPageUI != null)
        {
            startPageUI.UpdateGoldDisplay();
            Debug.Log("🔄 UpgradeTreeNode 刷新金币显示");
        }
    }

    /// <summary>
    ///     应用属性升级
    /// </summary>
    private void ApplyUpgrade()
    {
        if (upgradeType == UpgradeType.None || upgradeValue <= 0)
        {
            Debug.Log("ℹ️ 此节点不提供属性升级");
            return;
        }

        // 检查 GameManager 和 GameData 是否存在
        if (GameManager.Instance == null || GameManager.Instance.GameData == null)
        {
            Debug.LogError("❌ GameManager 或 GameData 为空，无法应用属性升级！");
            return;
        }

        switch (upgradeType)
        {
            case UpgradeType.MaxHealth:
                GameManager.Instance.GameData.maxHealth += upgradeValue;
                Debug.Log($"❤️ 最大生命值增加 {upgradeValue}，当前值：{GameManager.Instance.GameData.maxHealth}");
                break;
                
            case UpgradeType.AttackDamage:
                GameManager.Instance.GameData.attackDamage += upgradeValue;
                Debug.Log($"⚔️ 攻击力增加 {upgradeValue}，当前值：{GameManager.Instance.GameData.attackDamage}");
                break;
                
            case UpgradeType.MoveSpeed:
                GameManager.Instance.GameData.moveSpeed += upgradeValue;
                Debug.Log($"💨 移动速度增加 {upgradeValue}，当前值：{GameManager.Instance.GameData.moveSpeed}");
                break;
        }
        
        // 保存游戏数据
        GameManager.Instance.SaveGame();
    }

    /// <summary>
    ///     外部调用：尝试解锁（绑定到按钮点击事件）
    /// </summary>
    public void OnButtonClicked()
    {
        Debug.Log("点击按钮");
        // 检查是否可以解锁
        if (!canBeUnlocked)
        {
            Debug.LogWarning("⚠️ 前驱节点未解锁，无法解锁此节点！");
            return;
        }

        Unlock();
    }

    /// <summary>
    ///     手动设置解锁状态（用于测试或特殊逻辑）
    /// </summary>
    public void SetUnlocked(bool unlocked)
    {
        isLocked = !unlocked;
        SetLockedState(!unlocked);

        // 关闭自己的发光
        EnableGlow(false);
        
        // 通知后继节点更新状态
        if (unlocked && nextNodes != null)
            NotifySuccessors();
    }
}