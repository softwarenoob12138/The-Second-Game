using System.Collections;
using UnityEngine;

public class DeathBringerAttackState : EnemyState
{
    private Enemy_DeathBringer enemy;

    public DeathBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.chanceToTeleport += 5;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
        {
            if(enemy.CanTeleport())
            {
                stateMachine.ChangeState(enemy.teleportState);  //在Unity动画中添加的事件，触发该事件之后给triggerCalled赋值为true，然后进入另一个状态
            }
            else
            {
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}