using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//序列化，指将对象的状态转换为一种格式，而这种格式可以保存到文件，内存，或通过网络传输，反序列化就是将保存的数据恢复成对象
//使用 Serializable 标记该类可被序列化
[System.Serializable] 
public class Stat 
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
