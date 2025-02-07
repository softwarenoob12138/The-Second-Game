using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(4, null);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(4);

    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if( xInput == 0 || player.IsWallDetected() ) //移动时没输入或者碰到墙了都会切成待机状态
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
