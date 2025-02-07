using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// IPointerDownHandler ����ӿ��ܴ�������� UI_ItemSlot(������)�¼�
// IPointerEnterHandler ����ӿڵ������� UI_ItemSlot �����¼�
// IPointerExitHandler ������뿪 UI_ItemSlot ʱ�����¼�
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
                itemText.text = ""; //û�����ı����ǿյ� 
            }
        }
    }

    public void CleanUpSlot() //���ڸ�����Ʒ�� UI
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;

        itemText.text = "";
    }

     
    public virtual void OnPointerDown(PointerEventData eventData) //�������ʱ���õķ��� 
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
