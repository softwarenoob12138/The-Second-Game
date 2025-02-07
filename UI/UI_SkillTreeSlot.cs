using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;

    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBelocked;


    private void OnValidate()
    {
        gameObject.name = skillName;
    }

    private void Awake()
    {
        // 用 Awake 这个优先级比 Start 还高的方法
        // 避免 在其他技能管理脚本中 出现 此处的 unlock 还不为 true 的时候 就调用 其他技能管理脚本中技能解锁的判断语句
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if(unlocked)
        {
            skillImage.color = Color.white;
        }
    }

    public void UnlockSkillSlot()
    {
        if(PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
        {
            return;
        }

        for(int i = 0; i < shouldBeUnlocked.Length; i++)  // 前置 解锁
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                return;
            }
        }

        for(int i = 0; i < shouldBelocked.Length; i++) // 分支 解锁
        {
            if (shouldBelocked[i].unlocked == true)
            {
                return;
            }
        }

        unlocked = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription, skillName, skillCost);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value; 
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value)) // 检查是否存在这个键 如果存在则删除键 之后再添加新键值对
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
