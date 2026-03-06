using UnityEngine;

// 所有武器的基类 
public abstract class WeaponBase : MonoBehaviour 
{
    public WeaponData_SO weaponData; // 包含武器的图片、基础伤害等 
    protected float currentCooldown; 
    protected PlayerStats playerStats; // 引用玩家属性以计算最终伤害 

    // 动态初始化武器，取代直接在 Player 上挂载代码的旧方式
    // 新增一个开关，标记是否已经初始化
    private bool isInitialized = false; 

    public virtual void Initialize(WeaponData_SO data, PlayerStats stats) 
    {
        weaponData = data; 
        playerStats = stats; 
        currentCooldown = CalculateCooldown();
        
        // 数据喂饱了，打开开关！
        isInitialized = true; 
    }

    protected virtual void Update() 
    {
        // 【关键修复】如果还没被初始化，直接返回，不执行下面的逻辑
        if (!isInitialized) return; 

        currentCooldown -= Time.deltaTime; 
        if (currentCooldown <= 0) 
        {
            Attack(); 
            currentCooldown = CalculateCooldown();
        }
    }

    protected float CalculateCooldown()
    {
        return 1f / (weaponData.baseFireRate * playerStats.GetAttackSpeedMultiplier()); 
    }

    protected abstract void Attack(); 
}