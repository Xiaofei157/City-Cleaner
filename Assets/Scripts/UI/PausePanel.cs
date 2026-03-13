using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 暂停面板控制器
/// 挂载在 PausePanel 预制体上，负责暂停功能的 UI 交互
/// </summary>
public class PausePanel : MonoBehaviour
{
    [Header("按钮引用（自动查找或手动赋值）")]
    [SerializeField] private Button resumeButton;      // 继续游戏按钮
    [SerializeField] private Button mainMenuButton;    // 返回主界面按钮

    [Header("状态")]
    private bool isPaused = false;

    private void Awake()
    {
        // 如果没有手动赋值，尝试自动查找按钮
        if (resumeButton == null || mainMenuButton == null)
        {
            AutoFindButtons();
        }
    }

    private void Start()
    {
        // 绑定按钮事件
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClick);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
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
            // 根据按钮名称自动匹配
            if (btn.name.Contains("Resume") || btn.name.Contains("继续"))
            {
                if (resumeButton == null) resumeButton = btn;
            }
            else if (btn.name.Contains("MainMenu") || btn.name.Contains("返回") || btn.name.Contains("Main"))
            {
                if (mainMenuButton == null) mainMenuButton = btn;
            }
        }

        Debug.Log($"✅ PausePanelController 自动查找按钮完成 - 继续：{(resumeButton != null ? "✓" : "✗")} 返回：{(mainMenuButton != null ? "✓" : "✗")}");
    }

    /// <summary>
    /// 显示暂停面板
    /// </summary>
    public void ShowPausePanel()
    {
        isPaused = true;
        Time.timeScale = 0f; // 停止时间流逝
        gameObject.SetActive(true);
        Debug.Log("⏸️ 游戏已暂停");
    }

    /// <summary>
    /// 隐藏暂停面板并恢复游戏
    /// </summary>
    public void HidePausePanel()
    {
        isPaused = false;
        Time.timeScale = 1f; // 恢复时间流逝
        gameObject.SetActive(false);
        Debug.Log("▶️ 游戏已恢复");
    }

    /// <summary>
    /// 继续游戏按钮点击事件
    /// </summary>
    private void OnResumeClick()
    {
        HidePausePanel();
    }

    /// <summary>
    /// 返回主界面按钮点击事件 - 保存金币并返回
    /// </summary>
    private void OnMainMenuClick()
    {
        Debug.Log("💾 正在保存游戏数据并返回主界面...");
        
        // 确保金币数据已保存（将本局金币累加到总金币）
        if (PlayerStats.Instance != null)
        {
            // 调用 SaveGold 方法会自动将 sessionGold 累加到 totalGold
            // 这里直接保存 GameManager 的数据
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveGame();
                Debug.Log($"💰 金币已保存：总金币 = {PlayerStats.Instance.TotalGold}");
            }
        }

        // 恢复时间流速
        Time.timeScale = 1f;
        isPaused = false;
        
        // 隐藏面板
        gameObject.SetActive(false);

        // 加载主界面场景
        SceneManager.LoadScene("StartPage");
    }

    /// <summary>
    /// 清理按钮事件监听
    /// </summary>
    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClick);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
    }

    /// <summary>
    /// 应用程序退出时恢复时间流速并保存数据
    /// </summary>
    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
        
        // 退出前保存金币数据
        if (PlayerStats.Instance != null && GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log($"💾 退出游戏，金币已保存：总金币 = {PlayerStats.Instance.TotalGold}");
        }
    }

    /// <summary>
    /// 对象禁用时恢复时间流速（仅当处于暂停状态时）
    /// </summary>
    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
}
