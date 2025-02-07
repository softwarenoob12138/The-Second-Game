using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interface ����һ���ӿ� ���԰�������Ŀ��ʵ�ָ��õ�ģ�黯����չ��
// ���������� IPointerDownHandler �����Ѿ��� Unity ����õĽӿڣ�ֻ������ӿ������Լ������
public interface ISaveManager 
{
    void LoadData(GameData _data);
    void SaveData(ref GameData _data); // ref �������������ʱ���Ըı���
}
