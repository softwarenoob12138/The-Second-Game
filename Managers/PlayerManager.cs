using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager //这是C#中的 单例 ，即只有一个实例的方法(Player) ，并且有一个可全局访问的访问点
{
    public static PlayerManager instance;
    public Player player;    //在Unity引擎中分配这个player之后，就可以从任何关于player的脚本中访问Player组件了，就无需再用 GameObject.Find 这种会增加运行负担的命令了

    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);  //删除instance,即如果有多个实例的话，第一个将被赋值，其余的将被销毁，这就是C#中的 单例
        } 
        else
        { 
            instance = this;
        }
    }

    public bool HaveEnoughMoney(int _price)
    {
        if(_price > currency)
        {
            return false;
        }

        currency = currency - _price;
        return true;
    }

    public int GetCurrency() => currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
