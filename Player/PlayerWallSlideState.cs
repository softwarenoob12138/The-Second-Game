using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected() == false)
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;  //这里return之后就不会执行下面的这些命令，直接执行wallJump之后返回,避免下面的命令印象到此命令的一些变量设置
        }

        if (xInput != 0 && player.facingDir != xInput)  //这条语句是为了把角色从墙上拆下来
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (yInput < 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);
        }


        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}