using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType //用枚举给 Sword 赋予不同的功效
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{


    public SwordType swordType = SwordType.Regular; //给 Sword 的默认值就是 Regular，后续通过Unity更换类型

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private int pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;


    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }  //让其可以被其他脚本读取
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked { get; private set; }


    private Vector2 finalDir;

    [Header("Aim dots")]  //瞄准的参照点
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;  //点与点之间的间隙
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;
    private GameObject[] dots;



    protected override void Start()
    {
        base.Start();
        GenereateDots();
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnurable);
    }

    private void SetupGravity()  
    {
        if(swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if(swordType == SwordType.Spin)
        {
            swordGravity = spinGravity; 
        }
    } //其实这里用switch阅读性更强，看看后期能否改一改

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            //normalized (归一化向量) ，它返回一个单位向量，就是方向与原向量相同，但是长度(模)为1
            //结合一下就是说，给鼠标和玩家距离做一个单位向量
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y); 
            
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
            for(int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }

    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        //实例化一个新的 Sword_Skill_Controller 给这个新的实例里面的预设值赋完值以后，再给 Sword 使用这个新实例里面的方法
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>(); 


        if (swordType == SwordType.Bounce)  //如果 Sword 类型为 Bounce
        {
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed); //所有实现Bouncing的语句都是基于 isBouncing = true 的
        }
        else if(swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }
        else if(swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);
        //给launchForce.x和launchForce.y赋值，在松开Mouse1的时候，给以鼠标和玩家的单位向量方向做一个x轴上的力为launchForce.x，y轴上的力为launchForce.y的剑飞出去

        player.AssignNewSword(newSword); 

        DotsActive(false);
    }

    #region Unlock region

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPierceSword();
        UnlockTimeStop();
        UnlockVulnurable();
    }

    private void UnlockTimeStop()
    {
        if(timeStopUnlockButton.unlocked)
        {
            timeStopUnlocked = true;
        }
    }

    private void UnlockVulnurable()
    {
        if(vulnerableUnlockButton)
        {
            vulnerableUnlocked = true;
        }
    }

    private void UnlockSword()
    {
        if(swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;   
        }
    }

    private void UnlockBounceSword()
    {
        if(bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Bounce;
        }
    }

    private void UnlockPierceSword()
    {
        if(pierceUnlockButton.unlocked)
        {
            swordType = SwordType.Pierce;
        }
    }

    private void UnlockSpinSword()
    {
        if(spinUnlockButton.unlocked)
        {
            swordType = SwordType.Spin;
        }
    }

    #endregion

    #region Aim region
    public Vector2 AimDirection() 
    {
        Vector2 playerPosition = player.transform.position;

        //此函数可以帮助开发者处理用户的点击事件输入
        //当你在游戏中获取鼠标点击的位置或者触摸屏上的触摸点时，得到的是屏幕坐标系下的位置信息
        //值得注意的是，这个函数通常需要与场景中的主摄像机一起使用，所以要用 Camera.main
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);   
        Vector2 direction = mousePosition - playerPosition;  //获取鼠标和玩家之间的距离

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()  //产生瞄准点的函数
    {
        dots = new GameObject[numberOfDots];

        for(int i = 0; i < numberOfDots; i++)
        {
            //Instantiate 用于在运行时创建现有游戏对象的一个副本(实例化)
            //dotPrefab指定要实例化的预制体
            //player,transform.position 指定创建对象的位置
            //Quaternion.identity 表示创建的对象没有旋转
            //创建的新对象将为 dotsParent的子对象
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);

            dots[i].SetActive(false); //使处于非活跃状态

        }
    }

    private Vector2 DotsPosition(float t)  //根据物理知识里的斜抛运动来给这些点生成位置
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        
        return position;
    }
    #endregion

}
