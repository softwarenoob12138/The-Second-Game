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

        //  Color �����ĸ����������� ��(r)����(g)����(b)��͸����(a),ÿ��ֵ�ķ�Χ���Ǵ� 0 �� 1
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if(sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
        
    }

}
