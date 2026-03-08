using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    // 定义三种血条显示模式
    public enum DisplayMode 
    { 
        AlwaysOn,   // 一直显示
        ShowOnHit,  // 受击时显示一段时间
        AlwaysOff   // 一直不显示
    }

    [Header("显示设置")]
    public DisplayMode displayMode = DisplayMode.ShowOnHit;
    public float showDuration = 2f; // 受击后显示的时间

    [Header("UI 组件绑定")]
    public GameObject healthBarContainer; // 整个血条的父节点（用于整体隐藏/显示）
    public Image fillImage;               // 绿色的血条填充图
    public TextMeshProUGUI hpText;        // 显示数字的文本

    private float currentShowTimer = 0f;

    public void Initialize(float maxHP)
    {
        // 游戏刚开始或怪物刚生成时，根据模式决定初始状态
        UpdateVisibilityMode();
        UpdateHealth(maxHP, maxHP); // 初始满血
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        // 1. 更新血条比例
        if (fillImage != null) fillImage.fillAmount = currentHP / maxHP;
        
        // 2. 更新中间的数字 (向上取整，避免出现 0.5 血的情况)
        if (hpText != null) hpText.text = $"{Mathf.CeilToInt(currentHP)}/{Mathf.CeilToInt(maxHP)}";

        // 3. 处理受击显示逻辑
        if (displayMode == DisplayMode.ShowOnHit)
        {
            healthBarContainer.SetActive(true);
            currentShowTimer = showDuration; // 重置隐藏倒计时
        }
    }

    private void Update()
    {
        // 如果是“受击显示”模式，且倒计时正在进行
        if (displayMode == DisplayMode.ShowOnHit && currentShowTimer > 0)
        {
            currentShowTimer -= Time.deltaTime;
            if (currentShowTimer <= 0)
            {
                healthBarContainer.SetActive(false); // 时间到，隐藏血条
            }
        }
    }

    // 强制刷新血条的可见性状态（可以在游戏设置里调用它来一键切换所有怪物的血条模式）
    public void UpdateVisibilityMode()
    {
        if (healthBarContainer == null) return;

        switch (displayMode)
        {
            case DisplayMode.AlwaysOn:
                healthBarContainer.SetActive(true);
                break;
            case DisplayMode.AlwaysOff:
            case DisplayMode.ShowOnHit:
                healthBarContainer.SetActive(false);
                break;
        }
    }
}