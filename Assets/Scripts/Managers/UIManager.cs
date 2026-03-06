using UnityEngine;
using UnityEngine.UI; // 引入基础 UI 命名空间
using TMPro; // 推荐使用 TextMeshPro 来实现更清晰的文字显示

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("左上角：玩家状态")]
    public Slider healthBar; // 玩家自身的血条
    public Text levelText; // 玩家等级
    public Text healthText; // 新增：显示生命值数值（如 80/100）
    public Text xpText;     // 新增：显示经验值数值（如 15/50）

    [Header("底部：经验条")]
    public Slider xpBar;    // 新增：经验条进度

    [Header("右上角：资源")]
    public Text goldText; // 金币数量

    [Header("左下角：武器显示 (最多4个)")]
    public Image[] weaponSlots; // 存放4个武器图片的数组

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 更新玩家血条显示
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth; // Slider 的 value 范围是 0 到 1
        }
        
        // 新增：同时更新生命值数值文本
        if (healthText != null)
        {
            healthText.text = Mathf.FloorToInt(currentHealth) + "/" + Mathf.FloorToInt(maxHealth);
        }
    }

    // 更新等级显示
    public void UpdateLevel(int currentLevel)
    {
        if (levelText != null) levelText.text = "Lv." + currentLevel;
    }

    // 更新金币显示
    public void UpdateGold(int currentGold)
    {
        if (goldText != null) goldText.text = currentGold.ToString();
    }

    // 新增：更新经验值和进度条
    public void UpdateXP(int currentXP, int xpToNext)
    {
        // 更新经验条进度
        if (xpBar != null)
        {
            xpBar.value = (float)currentXP / xpToNext;
        }
        
        // 更新经验值文本
        if (xpText != null)
        {
            xpText.text = currentXP + "/" + xpToNext;
        }
    }

    // 更新左下角的武器图标显示
    public void UpdateWeaponSlots(WeaponData_SO[] equippedWeapons)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            // 如果该槽位有对应的已装备武器
            if (i < equippedWeapons.Length && equippedWeapons[i] != null)
            {
                weaponSlots[i].sprite = equippedWeapons[i].icon; // 读取 WeaponData_SO 中的专属图片
                weaponSlots[i].color = Color.white; // 确保颜色不透明
                weaponSlots[i].gameObject.SetActive(true); // 显示该槽位
            }
            else
            {
                weaponSlots[i].gameObject.SetActive(false); // 武器不足时隐藏多余槽位
            }
        }
    }
}