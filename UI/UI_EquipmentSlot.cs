using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    // �˷��� ÿ���ű������ֵ�����仯ʱ���õģ���ͨ�������ڱ༭���н���һЩ��֤���ʼ������
    // �������ȷ��ÿ���ڱ༭���и���ĳ�ֶε�ֵʱ��UI�������������Է�ӳ�µ���Ʒ��Ϣ
    private void OnValidate() 
    {
        gameObject.name = slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(item == null || item.data == null)
        {
            return;
        }

        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);
        CleanUpSlot();
        ui.itemTooltip.HideToolTip();
    }
}
