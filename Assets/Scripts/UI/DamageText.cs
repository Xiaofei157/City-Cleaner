using UnityEngine;
using TMPro; // 必须引入 TextMeshPro 命名空间

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;    // 向上飘的速度
    public float fadeSpeed = 2f;    // 变透明的速度
    public float lifeTime = 1f;     // 存活时间
    
    private float currentLifeTime;
    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        // 获取 TextMeshPro 组件 (注意不是 TextMeshProUGUI，因为我们要在世界空间显示)
        textMesh = GetComponent<TextMeshPro>();
    }

    // 当需要显示伤害时调用这个方法
    public void Setup(float damageAmount)
    {
        currentLifeTime = lifeTime;
        
        // 四舍五入，只显示整数伤害，看起来更干净
        textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
        
        // 重置颜色为完全不透明的白色（或者你喜欢的颜色，比如暴击可以是红色）
        textColor = textMesh.color;
        textColor.a = 1f;
        textMesh.color = textColor;
        
        // 稍微加一点随机的 X 轴偏移，防止数字完全重叠在一起
        float randomX = Random.Range(-0.5f, 0.5f);
        transform.position += new Vector3(randomX, 0, 0);
    }

    private void Update()
    {
        // 1. 向上移动
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. 逐渐变透明
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;

        // 3. 寿命结束，回收到对象池
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            if (PoolManager.Instance != null) PoolManager.Instance.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }
}