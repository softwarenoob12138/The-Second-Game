using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{

    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        
        defaultGravity = player.rb.gravityScale;

        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransprent(false);
        skillUsed = false;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 15);
        }

        if(stateTimer < 0)
        {
            rb.velocity = new Vector2 (0, -.1f);

            if(!skillUsed)
            {
                if(player.skill.blackhole.CanUseSkill())  // 这里 if 条件语句 调用了 CanUseSkill 方法，返回 true 之前调用了 UseSkill 使用了该技能
                {
                    skillUsed = true;
                }
            }
        }

        if(player.skill.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }

        /////////////////////////////////////////////////
        //在 Blackhole_Skill_Controller 里退出该状态/////
        /////////////////////////////////////////////////
    }
}
