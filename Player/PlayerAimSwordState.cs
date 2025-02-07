using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f); //以防结束此状态时立马进行移动出现角色原地平移的状态，所以给退出此状态增加些许后摇时间
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachine.ChangeState(player.idleState);
        }
        
        //此函数先前提到过,用于输入鼠标点击的位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
        {
            player.Flip();
        }
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
        {
            player.Flip();
        }
    }
}
