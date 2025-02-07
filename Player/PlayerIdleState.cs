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

        if (xInput == player.facingDir && player.IsWallDetected()) //������ǽ֮�󣬰�����ķ������ƶ������л���moveState
        {
            return;
        }

        if(xInput != 0 && !player.isBusy)  //���õ�Эͬ�����ڼ�ʱ����ǰ�޷�����moveState�ƶ�״̬
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
