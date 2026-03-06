using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    public Text nameText;
    public Image iconImage;
    public Text descText;
    //增加button组件
    private Button button;
    private UpgradeCard_SO cardData;
    private WeaponData_SO weaponData;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    // 初始化卡片显示（属性提升类）
    public void Setup(UpgradeCard_SO data)
    {
        cardData = data; 
        weaponData = null;
        
        if (nameText != null) nameText.text = data.cardName;
        if (iconImage != null) iconImage.sprite = data.icon;
        if (descText != null) descText.text = data.description;
    }

    // 初始化卡片显示（新武器类）
    public void SetupWeapon(WeaponData_SO data)
    {
        weaponData = data; 
        cardData = null;
        
        if (nameText != null) nameText.text = data.weaponName;
        if (iconImage != null) iconImage.sprite = data.icon;
        if (descText != null) descText.text = "新武器：" + data.description;
    }

    public void OnClick()
    {
        var manager = LevelUpManager.Instance;
        if (manager == null) return;

        if (cardData != null) manager.ApplyUpgrade(cardData);
        else if (weaponData != null) manager.ApplyNewWeapon(weaponData);
    }
}