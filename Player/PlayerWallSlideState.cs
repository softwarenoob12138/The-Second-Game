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
            return;  //����return֮��Ͳ���ִ���������Щ���ֱ��ִ��wallJump֮�󷵻�,�������������ӡ�󵽴������һЩ��������
        }

        if (xInput != 0 && player.facingDir != xInput)  //���������Ϊ�˰ѽ�ɫ��ǽ�ϲ�����
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