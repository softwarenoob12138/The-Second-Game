using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager //����C#�е� ���� ����ֻ��һ��ʵ���ķ���(Player) ��������һ����ȫ�ַ��ʵķ��ʵ�
{
    public static PlayerManager instance;
    public Player player;    //��Unity�����з������player֮�󣬾Ϳ��Դ��κι���player�Ľű��з���Player����ˣ����������� GameObject.Find ���ֻ��������и�����������

    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);  //ɾ��instance,������ж��ʵ���Ļ�����һ��������ֵ������Ľ������٣������C#�е� ����
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
