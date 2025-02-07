using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;  //图层消失速度
    
    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;


    private bool canDuplicateClone;
    private float chanceToDuplicate;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()  //克隆体被创建出来之后，给cloneTimer赋值以后开始计时并逐渐消失
    {
        cloneTimer -= Time.deltaTime;  

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if(sr.color.a <= 0 )
            {
                Destroy(gameObject);  //图层完全消失之后删除这个克隆
            }
        }
    }
    public void SetupClone(Transform _newTransform, float _cloneDuration , bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        if(_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));  //随机生成 1 2 3这三者之一
        }

        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        closestEnemy = _closestEnemy;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;  //在攻击动画结束之后马上给值降在0以下，让其攻击结束立马开始消失
    }

    private void AttackTrigger()
    {
        //Phisics2D 的OverlapCircleAll函数 获取两个变量分别是一个点和一个半径，然后构成的一个圆，让这个圆的大小作为碰撞器的大小
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        //如果进入的var变量落在这个碰撞器之中，就调用Damage函数
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //这里是检查是否有Enemy录入了这个碰撞器
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>()); 专门为克隆体的伤害制作一个函数

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier); 

                if(player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                    {
                        weaponData.Effect(hit.transform); // // 使克隆体可以触发玩家装备的武器的效果
                    }
                }

                if(canDuplicateClone)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        //每一个Clone都有一个Clone_Skill_Controller,默认 facingDir 为 1
                        //如果想明白了就会发现，每一个新的Clone实例都只会创造一个facingDir = -1的复制体
                        //复制体在攻击之后再进行判定是否创造一个新的 默认 facingDir 为 1的Clone
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()  //用于使克隆面向最近的目标
    {
        
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)  //如果克隆在最近的敌人的右边就会转向，否则不转向，这样就实现了克隆体可以面向最近的敌人
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
