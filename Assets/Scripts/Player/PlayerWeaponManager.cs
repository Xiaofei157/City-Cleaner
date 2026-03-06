using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public List<WeaponData_SO> equippedWeaponsData = new List<WeaponData_SO>();
    
    // 用于测试：在 Inspector 面板把扫把的数据拖进来
    public WeaponData_SO startingWeapon; 

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (startingWeapon != null)
        {
            EquipWeapon(startingWeapon);
        }
    }

    public void EquipWeapon(WeaponData_SO newWeaponData)
    {
        // 玩家最多可以装备4个武器 [cite: 29]
        if (equippedWeaponsData.Count >= 4) return; 

        // 根据预制体实例化武器 
        GameObject weaponObj = Instantiate(newWeaponData.prefab, transform.position, Quaternion.identity);
        
        // 将武器设置为玩家的子物体，这样它就会跟着玩家移动
       // weaponObj.transform.SetParent(this.transform);

        // 获取基类组件并唤醒它
        WeaponBase weaponComponent = weaponObj.GetComponent<WeaponBase>();
        if (weaponComponent != null)
        {
            // 关键：调用 Initialize 喂给它数据，解开它的“安全锁”！
            weaponComponent.Initialize(newWeaponData, playerStats);
            equippedWeaponsData.Add(newWeaponData);
            
            
            // 【UI修复：取消这里的注释，通知UI更新】
            if (UIManager.Instance != null) 
            {
                UIManager.Instance.UpdateWeaponSlots(equippedWeaponsData.ToArray());
            }
        
        }
    }
}