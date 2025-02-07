using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform; //RectTransform �̳��� Transform ��ר��Ϊ UI ��ƣ��ṩ�˸�������Ժͷ���������ê�㣬�߾࣬�ߴ��
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();  
        slider = GetComponentInChildren<Slider>();       

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    // ���������ʱ���õķ���
    private void OnEnable()
    {
        entity.onFlipped += FlipUI;  //ÿ�� onFlipped ��� Action ������ʱ ���� entity ��ȡ�� Entity ����� ���� FlipUI ����
        myStats.onHealthChanged += UpdateHealthUI;
    }

    // ���������ʱ���õķ���
    private void OnDisable()
    {
        if(entity != null)
        {
            entity.onFlipped -= FlipUI;
        }

        if(myStats != null)
        {
            myStats.onHealthChanged -= UpdateHealthUI;
        }

    }
    private void FlipUI() => myTransform.Rotate(0, 180, 0);
}
