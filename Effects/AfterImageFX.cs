using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;


    public void SetupAfterImage(float _loosingSpeed, Sprite _spriteImage)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = _spriteImage;

        colorLooseRate = _loosingSpeed;

    }

    private void Update()
    {
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;

        //  Color 类有四个浮点数参数 红(r)、绿(g)、蓝(b)和透明度(a),每个值的范围都是从 0 到 1
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if(sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
        
    }

}
