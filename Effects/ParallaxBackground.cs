using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;  //Ϊ�������õ��Ӳ�ű�:���Ǳ��������һ���ߣ����Ǳ��������������� ����һ����

    //�Ӳ�ЧӦ��ֵ�����˱������ƶ��ٶȺ���������ƶ��ٶȵĲ�࣬1������ȫ��ͬ�����٣�0.�����Ǳ��������
    [SerializeField] private float parallaxEffect;  

    private float xPosition;
    private float length;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x; //��ȡһ�ű����ĳ��� (������������ƴ�ӵ�)
        xPosition = transform.position.x;

    }

    private void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect); //��ȡ��������������Ӳ��ƶ��ľ���
        float distanceToMove = cam.transform.position.x * parallaxEffect;


        //��Ϊcamare��λ��һֱ�ڶ������Ա�����λ��Ҳ��һֱ�ڶ���
        transform.position = new Vector3(xPosition + distanceToMove, cam.transform.position.y);  //���ǹ��ڱ����ϵĽű������Ǳ�����λ�ñ仯   distansToMove�Ǵ��Ӳ���ƶ�ֵ

        if (distanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
        else if (distanceMoved < xPosition - length)
        {
            xPosition = xPosition - length;
        }
    }
}
