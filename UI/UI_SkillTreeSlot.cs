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
        // �� Awake ������ȼ��� Start ���ߵķ���
        // ���� ���������ܹ���ű��� ���� �˴��� unlock ����Ϊ true ��ʱ�� �͵��� �������ܹ���ű��м��ܽ������ж����
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

        for(int i = 0; i < shouldBeUnlocked.Length; i++)  // ǰ�� ����
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                return;
            }
        }

        for(int i = 0; i < shouldBelocked.Length; i++) // ��֧ ����
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
        if(_data.skillTree.TryGetValue(skillName,out bool value)) // ����Ƿ��������� ���������ɾ���� ֮��������¼�ֵ��
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
