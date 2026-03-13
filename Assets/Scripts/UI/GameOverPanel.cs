using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 结算面板管理器
/// 负责显示结算界面并处理返回主界面和重新开始功能
/// </summary>
public class GameOverPanel : MonoBehaviour
{
    [Header("按钮引用")]
    [SerializeField] private Button mainMenuButton;      // 返回主界面按钮
    [SerializeField] private Button restartButton;       // 重新开始按钮

    private void Awake()
    {
        // 自动查找按钮
        if (mainMenuButton == null || restartButton == null)
        {
            AutoFindButtons();
        }
    }

    /// <summary>
    /// 自动查找子物体中的按钮
    /// </summary>
    private void AutoFindButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        
        foreach (Button btn in buttons)
        {
            // 根据按钮名称匹配
            if (btn.name.Contains("Button") && btn.transform.childCount > 0)
            {
                Text buttonText = btn.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    if (buttonText.text.Contains("返回") || buttonText.text.Contains("主界面"))
                    {
                        mainMenuButton = btn;
                    }
                    else if (buttonText.text.Contains("重新"))
                    {
                        restartButton = btn;
                    }
                }
            }
        }

        Debug.Log($"✅ GameOverPanel 自动查找按钮完成 - 返回主界面：{(mainMenuButton != null ? "✓" : "✗")} 重新开始：{(restartButton != null ? "✓" : "✗")}");
    }

    private void Start()
    {
        // 绑定按钮事件
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClick);
        }
    }

    /// <summary>
    /// 返回主界面按钮点击事件
    /// </summary>
    private void OnMainMenuClick()
    {
        Debug.Log("💾 正在保存游戏数据并返回主界面...");
        
        // 保存金币数据
        if (PlayerStats.Instance != null && GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log($"💰 金币已保存：总金币 = {PlayerStats.Instance.TotalGold}");
        }

        // 恢复时间流速
        Time.timeScale = 1f;
        
        // 加载主界面场景
        SceneManager.LoadScene("StartPage");
    }

    /// <summary>
    /// 重新开始按钮点击事件
    /// </summary>
    private void OnRestartClick()
    {
        Debug.Log("🔄 正在重新开始游戏...");
        
        // 保存金币数据（将本局获得的金币累加到总金币）
        if (PlayerStats.Instance != null && GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log($"💰 金币已保存：总金币 = {PlayerStats.Instance.TotalGold}");
        }

        // 恢复时间流速
        Time.timeScale = 1f;
        
        // 重新加载当前场景
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// 清理按钮事件监听
    /// </summary>
    private void OnDestroy()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
        
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClick);
    }
}
