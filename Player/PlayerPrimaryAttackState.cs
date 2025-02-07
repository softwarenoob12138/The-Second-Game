using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float comboWindow = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(0, null);

        xInput = 0;  //������޸�attackDir��bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow) //������������֮�� ���� ���������һ�ι�����ʱ�䳬����comboWindow���ʹӵ�һ����ʼ
        {
            comboCounter = 0;
        }
        player.anim.SetInteger("ComboCounter", comboCounter);   //Integer = int  

        float attackDir = player.facingDir; //�����ڹ�����ʱ���л���������,��ǿ��ɫ�Ĳ���˳����

        if(xInput != 0)
        {
            attackDir = xInput; //BUG:  ������ڽ���״̬��ֵxInput = 0�Ļ���ͣ������ʱ��ͻ��¼��һ�ι���ʱ������attackDir�������һ�ε�attakcDir����һ�β�����ʱ������ͬ����������ͻ�Ͳ�����ʱ���෴
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir , player.attackMovement[comboCounter].y); //������Unity������ÿһ�ι����������ƶ�����(x���y���ϵ�) 

        stateTimer = .1f; //���������ȣ������ڹ���֮����һ�κ�ҡ���룬�ö���������������
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //���stateTimerһ��ʼ����С���㣬����ֱ�ӿ�����Ϊ��������ͣ�ƶ�ʱ�������ƶ��ٶȣ�����������state�͵�ͣ
        //��dashState״̬�У�Ҳ�õ���stateTimer�������丳ֵΪdashDuration>0,���Ա�֤dash״̬���޷����й���
        if (stateTimer < 0)  
        {
            player.SetZeroVelocity();
        }

        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
