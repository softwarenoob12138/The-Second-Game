using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning; //未赋值就是false  


    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("Pierce info")]
    private float pierceAmount;  //未赋值就是0，而切换成 pierceType 时才会给他赋 Unity 中设置的值

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;  //是位置运动停止的意思，不是停转
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }


    public void SetupSword(Vector2 _dir, float _gravityscale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityscale;

        if (pierceAmount <= 0)
        {
            anim.SetBool("Rotation", true);
        }


        Invoke("DestroyMe", 7); // Invoke 方法 在7秒后调用 DestroyMe 函数

        //Mathf.Clamp 限制 rb.velocity.x 的值在 -1 和 1 之间，如果大于 1 返回 1 ，如果小于 -1 返回 -1
        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;

        //给List<Transform>设置为private之前 值由Unity创建，设置为private之后没人创建了，也没有人给它默认空值，所以在下面设置一个新的列表来承载私有量的赋值
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravalDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravalDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        //rb.isKinematic = false;  
        //当此属性被设置为true时，这个物体将不再受到物理模拟的影响，如重力，碰撞等，但是它仍可以被其他非Kinematic物体或者脚本所控制移动
        //反之设置为false的时候就能收到物理模拟的影响

        rb.constraints = RigidbodyConstraints2D.FreezeAll; //为了让扔出去且尚未碰撞到任何碰撞体的剑能回来，就不采取此方式，而采取冻结所有坐标轴的方式
        transform.parent = null;
        isReturning = true;


        // 在这里做 剑 的冷却
    }

    private void Update()
    {
        if (canRotate)
        {
            //是将初速度赋给Unity引擎中移动目标模块中的x轴(移动时的x轴方向上的红色箭头)的正方向
            transform.right = rb.velocity;
        }

        if (isReturning)
        {  //MoveTowards函数,(从哪里开始，往哪里移动，以什么速度),使用这个方法时要用上 Vector2
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchTheSword();
            }
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;  //停转之后回来
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)  //用 hitCooldown 来设置 Damage() 间隔时间 每隔一段 hitCooldown 就 Dmage() 一下
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        //碰撞检测，看看命中(hit)的对象是否包含Enemy组件，如果确实包含这个组件，那么该对象的transform将被添加到enemyTarget列表中
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            //剑离一个弹射目标的距离小于 .1f 的时候，给索引 target++ 然后弹射到下一个目标，如此往复
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {

                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                bounceAmount--;

                if (bounceAmount < 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)  //制作小球吃金币的时候用到过，一种碰撞触发器,Collider2D collision是Unity引擎中的碰撞体类型和碰撞体名称
    {
        if (isReturning)
        {
            return;    // 如果剑正在返回时，是不会触发碰撞触发器的
        }

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);  //插入函数，如果 Sword 没有碰到 Enemy,就会插在 Ground 里面
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        
        player.stats.DoDamage(enemyStats);
                

        if(player.skill.sword.timeStopUnlocked)
        {
            enemy.FreezeTimeFor(freezeTimeDuration);
        }

        if(player.skill.sword.vulnerableUnlocked)
        {
            enemyStats.MakeVulnerableFor(freezeTimeDuration + 3);
        }

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        if (equipedAmulet != null)
        {
            equipedAmulet.Effect(enemy.transform);
        }
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null) //在 Sword 与 Enemy 碰撞后触发的一瞬间录入这个范围内的敌人
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);  //对于所有 hit 到 colliders中的变量
                                                         //hit 到的 Enemy 的 transform 加入到 enemyTarget 里去 
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (wasStopped)
        {
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();//碰到敌人时就停在原地旋转
            return;
        }

        canRotate = false;
        cd.enabled = false;  //让剑的碰撞器失效

        rb.isKinematic = true;  //让Body Type 变成 Kinematic
        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //让其所有坐标轴运动冻结
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;  //剑在弹射时不会停止旋转，也不会卡在别的父级上，插入到父级里去
        }

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;  //让其碰撞到的物体成为它的父级
    }
}
