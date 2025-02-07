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

        xInput = 0;  //用这个修复attackDir的bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow) //连击到第三下之后 或者 攻击后接下一次攻击的时间超过了comboWindow，就从第一击开始
        {
            comboCounter = 0;
        }
        player.anim.SetInteger("ComboCounter", comboCounter);   //Integer = int  

        float attackDir = player.facingDir; //可以在攻击的时候切换攻击方向,增强角色的操作顺畅度

        if(xInput != 0)
        {
            attackDir = xInput; //BUG:  如果不在进入状态赋值xInput = 0的话，停下来的时候就会记录上一次攻击时产生的attackDir，如果上一次的attakcDir和这一次不动的时候面向不同，攻击方向就会和不动的时候相反
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir , player.attackMovement[comboCounter].y); //可以在Unity中设置每一段攻击附带的移动距离(x轴和y轴上的) 

        stateTimer = .1f; //增加流畅度，让其在攻击之后有一段后摇距离，让动作更看起来流畅
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

        //这个stateTimer一开始就是小于零，所以直接可以作为条件，调停移动时攻击的移动速度，让其进入这个state就调停
        //在dashState状态中，也用到了stateTimer，并将其赋值为dashDuration>0,所以保证dash状态下无法进行攻击
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
