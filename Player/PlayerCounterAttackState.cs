using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{

    private bool canCreateClone;
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //如果进入的var变量落在这个碰撞器之中
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //这里是让获取的Enemy进入这个碰撞器
            {
                if(hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10;
                    player.anim.SetBool("SuccessfulCounterAttack", true);  //在这个动画里面有一个triggerCalled会返回true，就可以导致下面的if状态切换

                    player.skill.parry.UseSkill(); // 招架时 回复 生命值

                    if(canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.parry.MakeMirageOnParry(hit.transform);
                    }
                }
            }
        }
        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
