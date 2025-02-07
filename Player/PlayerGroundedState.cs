using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState  //���ְ������ܷ�������ű���
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

        //�����ջػ��ڷ��й����е� Sword �� ���� if ���ȵ��� HasNoSword()�������ȷ��� bool ֵ �����ж��Ƿ���� aimSword
        //�ڷ��� bool ֵ�Ĺ����� �����û�з��� true�����ͻ��� ���� ReturnSword() �����ٷ��� false������ false ���ٽ��� if �ж�
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

        if(!player.IsGroundDetected())  //״̬���������������£���Ҫ��һ��״ִ̬�������ִ����һ��״̬,����Ҫ��airState״ִ̬�������ִ����һ��״̬
        {
            stateMachine.ChangeState(player.airState);
        }

        //ֵ��һ����ǣ�������������ײ�����Ϊ�˱�����������ͷ��ʱ������ҵ�һ��״̬
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            //����������Զԭ�������ԭ����ǣ�״̬���������������£�����һ��KeyCode֮����Ҫ��һ��״ִ̬�������ִ����һ��״̬
            //Ҳ����˵�ڰ���һ��KeyCode֮����ΪjumpState�̳е���PlayerState���Բ�������Space��Ӱ��
            stateMachine.ChangeState(player.jumpState);
        }
    }
    
    private bool HasNoSword()
    {
        if(!player.sword)  //���player�����û���Ӽ�sword
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
