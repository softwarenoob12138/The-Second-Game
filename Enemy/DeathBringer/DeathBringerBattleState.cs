using UnityEngine;

public class DeathBringerBattleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private int moveDir;

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        // if (player.GetComponent<PlayerStats>().isDead)
        // {
        //     stateMachine.ChangeState(enemy.moveState);
        // }
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
                else
                {
                    stateMachine.ChangeState(enemy.idleState);
                }
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
