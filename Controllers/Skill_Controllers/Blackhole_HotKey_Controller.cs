using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;  //����Unity��һ���ǳ�ǿ���ı���Ⱦ��� using TMPro ����ʹ��

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy; 
        blackhole = _myBlackHole;

        myHotKey = _myNewHotKey;   //��ÿһ�� SetupHotKey ֮�󣬶���һ���µ� _myNewHotKey ���洢�� myHotKey �У����ᱻ Remove ��
        myText.text = myHotKey.ToString();

    }


    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))   
        {
            blackhole.AddEnemyToList(myEnemy);

            myText.color = Color.clear; //����ɫ����Ϊ��ȫ͸��
            sr.color = Color.clear;
        }
    }


}
