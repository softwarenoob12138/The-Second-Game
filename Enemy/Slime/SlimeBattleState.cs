using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBattleState : EnemyState
{
    private Enemy_Slime enemy;
    private Transform player;
    private int moveDir;

    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();   //进入这个BattleState之后才会一直按下面的语句跟着玩家走

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance) //enemy.IsPlayerDetected().distance 是与玩家的距离
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
        }
        else  //在丢失玩家视野之后才执行的这个稍后进入待机状态的命令
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 7)  //Distance函数用法就是获取两个变量之间的距离,使用时要带上 Vector2
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        if (player.position.x > enemy.transform.position.x) //意味着怪站在玩家的左边
        {
            moveDir = 1;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && enemy.IsPlayerDetected())
            {
                return;
            }
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        }

        else if (player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && enemy.IsPlayerDetected())
            {
                return;
            }

            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        }

    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

}
