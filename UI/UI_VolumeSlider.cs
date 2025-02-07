using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parameter;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;

    public void SliderValue(float _value)
    {
        // Log10 这是以10为底的对数函数
        // 使用对数和转换是因为人耳对声音的感知是非线性的，使用对数值可以使音量变化听起来更加自然
        audioMixer.SetFloat(parameter, Mathf.Log10(_value) * multiplier);
    }

    public void LoadSlider(float _value)
    {
        if(_value >= 0.001f)
        {
            slider.value = _value;
        }
    }

}
