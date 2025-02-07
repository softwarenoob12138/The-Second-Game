using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea] // ������Inspector ������Զ����ı������ʽ��ʾ�ַ����ֶ�
    [SerializeField] private string statDescription;

    private void OnValidate()  // OnValidate() ���� �� Unity �ж�̬���±������
    {
        gameObject.name = statName;

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
    }
    private void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();  //��Ҫע����� statValueText.text �� string ���ͱ���

            if(statType == StatType.Health)
            {
                statValueText.text = playerStats.GetMaxHealthValue().ToString();
            }

            if(statType == StatType.damage)
            {
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
            }

            if(statType == StatType.critPower)
            {
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();
            }

            if(statType == StatType.critChance)
            {
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();
            }

            if(statType == StatType.evasion)
            {
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
            }

            if (statType == StatType.magicResistance)
            {
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statTooltip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideStatToolTip();
    }
}
