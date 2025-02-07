using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState  //各种按键功能放在这个脚本里
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackHoleUnlocked)
        {
            if(player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown!");
                return;
            }

            stateMachine.ChangeState(player.blackHole);
        }

        //这里收回还在飞行过程中的 Sword ， 这里 if 是先调用 HasNoSword()，让其先返回 bool 值 再做判断是否进入 aimSword
        //在返回 bool 值的过程中 ，如果没有返回 true，他就会先 调用 ReturnSword() 函数再返回 false，返回 false 后再进行 if 判断
        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked) 
        {
            stateMachine.ChangeState(player.aimSword);
        }

        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
        {
            stateMachine.ChangeState(player.counterAttack);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.primaryAttack);
        }

        if(!player.IsGroundDetected())  //状态机在这样的条件下，需要将一个状态执行完才能执行下一个状态,这里要等airState状态执行完才能执行下一个状态
        {
            stateMachine.ChangeState(player.airState);
        }

        //值得一提的是，这里加入地面碰撞检查是为了避免跳到敌人头上时触发玩家的一下状态
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            //不会连按跳远原地升天的原因就是，状态机在这样的条件下，按了一下KeyCode之后需要将一个状态执行完才能执行下一个状态
            //也就是说在按了一下KeyCode之后，因为jumpState继承的是PlayerState所以不受连按Space的影响
            stateMachine.ChangeState(player.jumpState);
        }
    }
    
    private bool HasNoSword()
    {
        if(!player.sword)  //如果player组件下没有子级sword
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
