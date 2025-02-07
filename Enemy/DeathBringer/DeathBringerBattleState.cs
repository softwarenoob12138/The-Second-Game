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
        base.Update();   //�������BattleState֮��Ż�һֱ������������������

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance) //enemy.IsPlayerDetected().distance ������ҵľ���
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

        if (player.position.x > enemy.transform.position.x) //��ζ�Ź�վ����ҵ����
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
