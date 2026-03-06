using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式，允许全局访问
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 确保场景中只有一个 GameManager 实例
        if (Instance == null)
        {
            Instance = this;
            // 切换场景时不要销毁这个核心管理器
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("GameManager 初始化成功！准备开始清理城市！");
    }
}