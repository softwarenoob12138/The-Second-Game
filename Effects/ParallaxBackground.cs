using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;  //为背景设置的视差脚本:就是背景摄像机一起走，但是背景的相对于摄像机 会慢一点走

    //视差效应的值决定了背景的移动速度和摄像机的移动速度的差距，1就是完全相同的移速，0.几就是比摄像机慢
    [SerializeField] private float parallaxEffect;  

    private float xPosition;
    private float length;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x; //获取一张背景的长度 (背景是由三张拼接的)
        xPosition = transform.position.x;

    }

    private void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect); //获取背景和摄像机的视差移动的距离
        float distanceToMove = cam.transform.position.x * parallaxEffect;


        //因为camare的位置一直在动，所以背景的位置也是一直在动的
        transform.position = new Vector3(xPosition + distanceToMove, cam.transform.position.y);  //这是挂在背景上的脚本，这是背景的位置变化   distansToMove是带视差的移动值

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
