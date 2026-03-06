using UnityEngine;

// 故障的清洁机器人 (BOSS)：召唤小弟，体型巨大
public class BrokenBotAI : EnemyAI
{
    [Header("BOSS 召唤技能")]
    public GameObject minionPrefab;   // 拖入之前做好的“垃圾虫”预制体
    public float summonCooldown = 6f; // 每6秒召唤一波
    public int summonCount = 3;       // 每次召唤3只

    private float timer;

    protected override void Update()
    {
        base.Update(); // BOSS 一直追踪玩家

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SummonMinions();
            timer = summonCooldown;
        }
    }

    private void SummonMinions()
    {
        if (PoolManager.Instance == null || minionPrefab == null) return;

        // 在 BOSS 周围一圈召唤小怪
        for (int i = 0; i < summonCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 2f; // 在半径2的圆上生成
            Vector3 spawnPos = transform.position + (Vector3)randomOffset;

            GameObject minion = PoolManager.Instance.Get(minionPrefab);
            minion.transform.position = spawnPos;
            
            // 重新初始化小怪 (需要读取小怪的数据，如果你的预制体本身配好了可以在对象池生成时自动处理)
            EnemyAI minionAI = minion.GetComponent<EnemyAI>();
            // 注意：如果你的垃圾虫预制体需要EnemyData_SO才能行动，请确保此处正确传入！
        }
        
        // Debug.Log("BOSS 故障清洁机器人召唤了垃圾虫小队！");
    }
}