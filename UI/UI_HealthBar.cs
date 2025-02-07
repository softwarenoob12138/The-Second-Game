using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform; //RectTransform 继承自 Transform 但专门为 UI 设计，提供了更多的属性和方法来处理锚点，边距，尺寸等
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

    // 该组件启用时调用的方法
    private void OnEnable()
    {
        entity.onFlipped += FlipUI;  //每当 onFlipped 这个 Action 被调用时 就往 entity 获取的 Entity 组件中 加入 FlipUI 方法
        myStats.onHealthChanged += UpdateHealthUI;
    }

    // 该组件禁用时调用的方法
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
