using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;  //它是Unity中一个非常强的文本渲染组件 using TMPro 才能使用

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy; 
        blackhole = _myBlackHole;

        myHotKey = _myNewHotKey;   //在每一个 SetupHotKey 之后，都有一个新的 _myNewHotKey 被存储在 myHotKey 中，不会被 Remove 掉
        myText.text = myHotKey.ToString();

    }


    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))   
        {
            blackhole.AddEnemyToList(myEnemy);

            myText.color = Color.clear; //将颜色设置为完全透明
            sr.color = Color.clear;
        }
    }


}
