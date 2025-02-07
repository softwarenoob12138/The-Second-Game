using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput == player.facingDir && player.IsWallDetected()) //这里碰墙之后，按朝向的反方向移动才能切换到moveState
        {
            return;
        }

        if(xInput != 0 && !player.isBusy)  //设置的协同程序在计时结束前无法进入moveState移动状态
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
