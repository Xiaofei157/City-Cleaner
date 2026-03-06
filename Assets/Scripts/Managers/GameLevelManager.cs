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
    public Transform playerTransform;   // 玩家的位置（用于计算刷怪点，在玩家周围生成）
    private float spawnTimer = 0f;      // 刷怪计时器，记录距离上次刷怪过去了多久
    public float spawnInterval = 1f; // 默认1秒刷一只

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // 简单的波次时间轴控制 (例如：0-60秒刷垃圾虫，300秒BOSS战)
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        // 随机选择一个怪物种类
        EnemyData_SO enemyToSpawn = availableEnemies[Random.Range(0, availableEnemies.Count)];

        // 核心规则：判断该怪物是否允许在当前关卡出现
        if (currentStageLevel >= enemyToSpawn.minLevelAppearance && currentStageLevel <= enemyToSpawn.maxLevelAppearance)
        {
            SpawnAtEdge(enemyToSpawn);
        }
    }

    // 边缘生成算法：获取相机视野外的随机点 
    private void SpawnAtEdge(EnemyData_SO enemyData)
    {
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