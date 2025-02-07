using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType //��ö�ٸ� Sword ���費ͬ�Ĺ�Ч
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{


    public SwordType swordType = SwordType.Regular; //�� Sword ��Ĭ��ֵ���� Regular������ͨ��Unity��������

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
    public bool swordUnlocked { get; private set; }  //������Ա������ű���ȡ
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

    [Header("Aim dots")]  //��׼�Ĳ��յ�
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;  //�����֮��ļ�϶
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
    } //��ʵ������switch�Ķ��Ը�ǿ�����������ܷ��һ��

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            //normalized (��һ������) ��������һ����λ���������Ƿ�����ԭ������ͬ�����ǳ���(ģ)Ϊ1
            //���һ�¾���˵����������Ҿ�����һ����λ����
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
        //ʵ����һ���µ� Sword_Skill_Controller ������µ�ʵ�������Ԥ��ֵ����ֵ�Ժ��ٸ� Sword ʹ�������ʵ������ķ���
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>(); 


        if (swordType == SwordType.Bounce)  //��� Sword ����Ϊ Bounce
        {
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed); //����ʵ��Bouncing����䶼�ǻ��� isBouncing = true ��
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
        //��launchForce.x��launchForce.y��ֵ�����ɿ�Mouse1��ʱ�򣬸���������ҵĵ�λ����������һ��x���ϵ���ΪlaunchForce.x��y���ϵ���ΪlaunchForce.y�Ľ��ɳ�ȥ

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

        //�˺������԰��������ߴ����û��ĵ���¼�����
        //��������Ϸ�л�ȡ�������λ�û��ߴ������ϵĴ�����ʱ���õ�������Ļ����ϵ�µ�λ����Ϣ
        //ֵ��ע����ǣ��������ͨ����Ҫ�볡���е��������һ��ʹ�ã�����Ҫ�� Camera.main
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);   
        Vector2 direction = mousePosition - playerPosition;  //��ȡ�������֮��ľ���

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()  //������׼��ĺ���
    {
        dots = new GameObject[numberOfDots];

        for(int i = 0; i < numberOfDots; i++)
        {
            //Instantiate ����������ʱ����������Ϸ�����һ������(ʵ����)
            //dotPrefabָ��Ҫʵ������Ԥ����
            //player,transform.position ָ�����������λ��
            //Quaternion.identity ��ʾ�����Ķ���û����ת
            //�������¶���Ϊ dotsParent���Ӷ���
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);

            dots[i].SetActive(false); //ʹ���ڷǻ�Ծ״̬

        }
    }

    private Vector2 DotsPosition(float t)  //��������֪ʶ���б���˶�������Щ������λ��
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        
        return position;
    }
    #endregion

}
