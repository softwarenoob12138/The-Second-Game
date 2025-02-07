using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// IPointerDownHandler 这个接口能处理鼠标点击 UI_ItemSlot(或触摸屏)事件
// IPointerEnterHandler 这个接口当鼠标进入 UI_ItemSlot 触发事件
// IPointerExitHandler 当鼠标离开 UI_ItemSlot 时触发事件
public class UI_ItemSlot : MonoBehaviour , IPointerDownHandler ,IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();    
    }

    public void UpdateSlot(InventoryItem _newitem)
    {
        item = _newitem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = ""; //没东西文本就是空的 
            }
        }
    }

    public void CleanUpSlot() //用于更新物品栏 UI
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;

        itemText.text = "";
    }

     
    public virtual void OnPointerDown(PointerEventData eventData) //当鼠标点击时调用的方法 
    {
        if(item == null)
        {
            return;
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            ui.itemTooltip.HideToolTip();
            return;
        }

        if(item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
            ui.itemTooltip.HideToolTip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null)
        {
            return;
        }

        ui.itemTooltip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        ui.itemTooltip.HideToolTip();
    }
}
