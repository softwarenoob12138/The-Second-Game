using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // OnBeforeSerialize 方法 在序列化之前(Unity 将对象的数据保存到磁盘之前，从Play模式退出时)被调用
    // 清空keys 和 values列表后，将字典中的键值对分别添加到 keys 和 values 列表中
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(KeyValuePair<TKey,TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // OnAfterDeserialize 方法 在反序列化之后(Unity 从磁盘加载对象的数据之后，Play模式启动时)被调用
    // 清空字典后，将 keys 和 values 列表中的数据重新添加到字典中
    public void OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {

        }

        for(int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

}
