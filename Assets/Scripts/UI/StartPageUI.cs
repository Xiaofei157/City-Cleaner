using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPageUI : MonoBehaviour
{
    [Header("按钮引用")]
    [SerializeField] private Button startGameButton;      // 开始游戏按钮
    [SerializeField] private Button upgradeTreeButton;   // 成长树按钮
    [SerializeField] private Button gameRulesButton;     // 游戏规则按钮
    [SerializeField] private Button gameSettingsButton;  // 游戏设置按钮
    [SerializeField] private Button exitGameButton;      // 退出游戏按钮
    [SerializeField] private Button addPageConfirmButton; // Add Page 确定按钮
    [SerializeField] private Button addPageBackButton;    // Add Page 返回按钮

    [Header("页面物体引用")]
    [SerializeField] private GameObject addPage;         // Add Page 页面
    [SerializeField] private GameObject gameRulesPage;   // Game Rules 页面
    [SerializeField] private GameObject gameSettingsPage; // Game Settings 页面

    [Header("金币显示")]
    [SerializeField] private Text goldText;              // 金币数量文本

    // Start is called before the first frame update
    void Start()
    {
        // 初始化所有页面为隐藏状态
        HideAllPages();

        // 绑定按钮事件
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClick);
        }

        if (upgradeTreeButton != null)
        {
            upgradeTreeButton.onClick.AddListener(OnUpgradeTreeClick);
        }

        if (gameRulesButton != null)
        {
            gameRulesButton.onClick.AddListener(OnGameRulesClick);
        }

        if (gameSettingsButton != null)
        {
            gameSettingsButton.onClick.AddListener(OnGameSettingsClick);
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameClick);
        }

        // 绑定 Add Page 按钮事件
        if (addPageConfirmButton != null)
        {
            addPageConfirmButton.onClick.AddListener(OnAddPageConfirmClick);
        }

        if (addPageBackButton != null)
        {
            addPageBackButton.onClick.AddListener(OnAddPageBackClick);
        }

        // 初始化金币显示
        UpdateGoldDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 更新金币显示（成长树中显示总金币）
    /// </summary>
    public void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            if (PlayerStats.Instance != null)
            {
                // 成长树界面显示玩家的总金币
                int totalGold = PlayerStats.Instance.TotalGold;
                goldText.text = totalGold.ToString();
                Debug.Log($"💰 成长树金币更新：{totalGold} (本局：{PlayerStats.Instance.sessionGold})");
            }
            else
            {
                // PlayerStats 未初始化时，尝试直接从 GameManager 读取
                if (GameManager.Instance != null && GameManager.Instance.GameData != null)
                {
                    int totalGold = GameManager.Instance.GameData.gold;
                    goldText.text = totalGold.ToString();
                    Debug.Log($"💰 成长树金币更新（直接读取）：{totalGold}");
                }
                else
                {
                    goldText.text = "0";
                    Debug.LogWarning("⚠️ 无法更新金币显示：PlayerStats.Instance 和 GameManager.Instance 均为空");
                }
            }
        }
        else
        {
            Debug.LogError("❌ goldText 引用为空！请在 Inspector 中赋值");
        }
    }

    /// <summary>
    /// 显示金币不足提示
    /// </summary>
    public void ShowInsufficientGoldMessage(int requiredGold)
    {
        Debug.LogWarning($"⚠️ 金币不足！需要 {requiredGold} 金币，当前金币不足。");
        // 这里可以扩展为弹出 UI 提示框
        // 例如：ShowMessageBox($"金币不足！\n需要 {requiredGold} 金币");
    }

    /// <summary>
    /// 开始游戏按钮点击事件 - 跳转到 SampleScene 场景
    /// </summary>
    private void OnStartGameClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// 隐藏所有页面
    /// </summary>
    private void HideAllPages()
    {
        if (addPage != null) addPage.SetActive(false);
        if (gameRulesPage != null) gameRulesPage.SetActive(false);
        if (gameSettingsPage != null) gameSettingsPage.SetActive(false);
    }

    /// <summary>
    /// 成长树按钮点击事件 - 显示 Add Page 并更新金币显示
    /// </summary>
    private void OnUpgradeTreeClick()
    {
        HideAllPages();
        if (addPage != null) 
        {
            addPage.SetActive(true);
            // 每次打开 Add Page 时都强制刷新金币显示
            UpdateGoldDisplay();
            Debug.Log("🌳 打开成长树，刷新金币显示");
        }
    }

    /// <summary>
    /// 游戏规则按钮点击事件 - 显示 Game Rules
    /// </summary>
    private void OnGameRulesClick()
    {
        HideAllPages();
        if (gameRulesPage != null) gameRulesPage.SetActive(true);
    }

    /// <summary>
    /// 游戏设置按钮点击事件 - 显示 Game Settings
    /// </summary>
    private void OnGameSettingsClick()
    {
        HideAllPages();
        if (gameSettingsPage != null) gameSettingsPage.SetActive(true);
    }

    /// <summary>
    /// 退出游戏按钮点击事件
    /// </summary>
    private void OnExitGameClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Add Page 确定按钮点击事件 - 隐藏 Add Page 并刷新主界面金币
    /// </summary>
    private void OnAddPageConfirmClick()
    {
        // 保存游戏数据（确保成长树的加点数据被保存）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log("💾 Add Page 确定，已保存成长树加点数据");
        }
        
        HideAllPages();
        // 关闭成长树页面后，刷新主界面的金币显示
        UpdateGoldDisplay();
        Debug.Log("✅ Add Page 确定，已关闭页面并刷新金币");
    }

    /// <summary>
    /// Add Page 返回按钮点击事件 - 隐藏 Add Page 并刷新主界面金币
    /// </summary>
    private void OnAddPageBackClick()
    {
        // 保存游戏数据（确保成长树的加点数据被保存）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log("💾 Add Page 返回，已保存成长树加点数据");
        }
        
        HideAllPages();
        // 关闭成长树页面后，刷新主界面的金币显示
        UpdateGoldDisplay();
        Debug.Log("✅ Add Page 返回，已关闭页面并刷新金币");
    }

    /// <summary>
    /// 清理按钮事件监听（可选）
    /// </summary>
    private void OnDestroy()
    {
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClick);
        
        if (upgradeTreeButton != null)
            upgradeTreeButton.onClick.RemoveListener(OnUpgradeTreeClick);
        
        if (gameRulesButton != null)
            gameRulesButton.onClick.RemoveListener(OnGameRulesClick);
        
        if (gameSettingsButton != null)
            gameSettingsButton.onClick.RemoveListener(OnGameSettingsClick);
        
        if (exitGameButton != null)
            exitGameButton.onClick.RemoveListener(OnExitGameClick);
        
        if (addPageConfirmButton != null)
            addPageConfirmButton.onClick.RemoveListener(OnAddPageConfirmClick);
        
        if (addPageBackButton != null)
            addPageBackButton.onClick.RemoveListener(OnAddPageBackClick);
    }
}
