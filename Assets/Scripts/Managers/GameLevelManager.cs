using UnityEngine;
using System.Collections.Generic;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }

    [Header("游戏进度")]
    public int currentStageLevel = 1; // 当前处于第几关
    public float gameTimer = 0f;      // 记录当前游戏时间

    [Header("刷怪配置")]
    public List<EnemyData_SO> availableEnemies; // 拖入第二阶段做好的怪物数据
    public List<EnemyData_SO> specialEnemies;   // 特殊怪物列表（BOSS、精英等）
    public Transform playerTransform;   // 玩家的位置（用于计算刷怪点，在玩家周围生成）
    private float spawnTimer = 0f;      // 刷怪计时器，记录距离上次刷怪过去了多久
    public float spawnInterval = 1f; // 默认 1 秒刷一只
        
    [Header("特殊怪物生成设置")]
    public float bossSpawnTime = 300f;    // BOSS 出现时间（秒）
    public float eliteSpawnInterval = 60f; // 精英怪出现间隔（秒）
    private bool bossSpawned = false;     // 记录 BOSS 是否已生成
    private float lastEliteSpawnTime = 0f; // 记录上次生成精英怪的时间

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
    
        // 普通怪物生成
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
            
        // 特殊怪物生成检查
        CheckSpecialEnemySpawn();
    }
        
    /// <summary>
    /// 检查并生成特殊怪物（BOSS、精英）
    /// </summary>
    private void CheckSpecialEnemySpawn()
    {
        // 生成 BOSS
        if (!bossSpawned && gameTimer >= bossSpawnTime)
        {
            SpawnBoss();
            bossSpawned = true;
        }
            
        // 生成精英怪
        if (gameTimer - lastEliteSpawnTime >= eliteSpawnInterval)
        {
            SpawnElite();
            lastEliteSpawnTime = gameTimer;
        }
    }
        
    /// <summary>
    /// 生成 BOSS
    /// </summary>
    private void SpawnBoss()
    {
        // 检查特殊怪物列表是否为空
        if (specialEnemies == null || specialEnemies.Count == 0)
        {
            Debug.LogWarning("⚠️ SpecialEnemies 列表为空，请配置 BOSS 和精英怪数据！");
            return;
        }
        
        // 从特殊怪物列表中找出所有 BOSS
        List<EnemyData_SO> bosses = specialEnemies.FindAll(e => e.isBoss);
            
        if (bosses.Count == 0) return;
            
        // 选择一个符合当前关卡的 BOSS
        foreach (var boss in bosses)
        {
            if (CanSpawnSpecialEnemy(boss))
            {
                Debug.Log($"⚠️ BOSS 战开始！{boss.enemyName} 出现了！");
                SpawnAtEdge(boss, true); // isBoss = true
                break;
            }
        }
    }
        
    /// <summary>
    /// 生成精英怪
    /// </summary>
    private void SpawnElite()
    {
        // 检查特殊怪物列表是否为空
        if (specialEnemies == null || specialEnemies.Count == 0)
        {
            return;
        }
        
        // 从特殊怪物列表中找出所有精英怪
        List<EnemyData_SO> elites = specialEnemies.FindAll(e => e.isElite);
            
        if (elites.Count == 0) return;
            
        // 选择一个符合当前关卡的精英怪
        foreach (var elite in elites)
        {
            if (CanSpawnSpecialEnemy(elite))
            {
                Debug.Log($"💪 精英怪出现！{elite.enemyName}");
                SpawnAtEdge(elite, true); // isBoss = true
                break;
            }
        }
    }
        
    /// <summary>
    /// 判断特殊怪物是否可以生成
    /// </summary>
    private bool CanSpawnSpecialEnemy(EnemyData_SO enemy)
    {
        // 如果指定了特定关卡，则只在这些关卡生成
        if (enemy.specificLevels != null && enemy.specificLevels.Count > 0)
        {
            return enemy.specificLevels.Contains(currentStageLevel);
        }
            
        // 否则按照 min/max Level 规则
        return currentStageLevel >= enemy.minLevelAppearance && 
               currentStageLevel <= enemy.maxLevelAppearance;
    }

    private void TrySpawnEnemy()
    {
        // 检查普通怪物列表是否为空
        if (availableEnemies == null || availableEnemies.Count == 0)
        {
            Debug.LogWarning("⚠️ AvailableEnemies 列表为空，请配置普通怪物数据！");
            return;
        }
        
        // 随机选择一个怪物种类
        EnemyData_SO enemyToSpawn = availableEnemies[Random.Range(0, availableEnemies.Count)];

        // 核心规则：判断该怪物是否允许在当前关卡出现
        if (currentStageLevel >= enemyToSpawn.minLevelAppearance && 
            currentStageLevel <= enemyToSpawn.maxLevelAppearance)
        {
            // 排除已经在特殊怪物列表中的怪物（避免重复生成）
            if (!specialEnemies.Contains(enemyToSpawn))
            {
                SpawnAtEdge(enemyToSpawn, false);
            }
        }
    }

    // 边缘生成算法：获取相机视野外的随机点 
    private void SpawnAtEdge(EnemyData_SO enemyData, bool isImportant = false)
    {
        // 检查 Camera.main 是否存在
        if (Camera.main == null)
        {
            Debug.LogError("❌ 场景中未找到主相机（Main Camera）！无法生成怪物。");
            return;
        }
        
        Vector2 viewportPoint = new Vector2(Random.value, Random.value);
        if (Random.value > 0.5f) 
            viewportPoint.x = (Random.value > 0.5f) ? 1.1f : -0.1f;
        else 
            viewportPoint.y = (Random.value > 0.5f) ? 1.1f : -0.1f;

        Vector2 spawnPos = Camera.main.ViewportToWorldPoint(viewportPoint);

        // 【更新这里】通过 PoolManager 获取怪物
        if (PoolManager.Instance != null && enemyData.prefab != null)
        {
            GameObject enemyObj = PoolManager.Instance.Get(enemyData.prefab);
            enemyObj.transform.position = spawnPos;
            
            // 初始化怪物属性并开始追踪玩家
            EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Initialize(enemyData);
            }
        }
    }
    
}
