using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interface 定义一个接口 可以帮助在项目中实现更好的模块化和扩展性
// 就是类似于 IPointerDownHandler 这种已经被 Unity 定义好的接口，只是这个接口是你自己定义的
public interface ISaveManager 
{
    void LoadData(GameData _data);
    void SaveData(ref GameData _data); // ref 允许你调用数据时可以改变它
}
