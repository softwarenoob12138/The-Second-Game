using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    // 此方法 每当脚本组件的值发生变化时调用的，这通常用于在编辑器中进行一些验证或初始化操作
    // 这里可以确保每次在编辑器中更改某字段的值时，UI都会立即更新以反映新的物品信息
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
