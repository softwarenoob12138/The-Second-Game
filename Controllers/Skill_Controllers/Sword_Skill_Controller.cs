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
    private bool isReturning; //δ��ֵ����false  


    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("Pierce info")]
    private float pierceAmount;  //δ��ֵ����0�����л��� pierceType ʱ�Ż������ Unity �����õ�ֵ

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
    private bool wasStopped;  //��λ���˶�ֹͣ����˼������ͣת
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


        Invoke("DestroyMe", 7); // Invoke ���� ��7������ DestroyMe ����

        //Mathf.Clamp ���� rb.velocity.x ��ֵ�� -1 �� 1 ֮�䣬������� 1 ���� 1 �����С�� -1 ���� -1
        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;

        //��List<Transform>����Ϊprivate֮ǰ ֵ��Unity����������Ϊprivate֮��û�˴����ˣ�Ҳû���˸���Ĭ�Ͽ�ֵ����������������һ���µ��б�������˽�����ĸ�ֵ
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
        //�������Ա�����Ϊtrueʱ��������彫�����ܵ�����ģ���Ӱ�죬����������ײ�ȣ��������Կ��Ա�������Kinematic������߽ű��������ƶ�
        //��֮����Ϊfalse��ʱ������յ�����ģ���Ӱ��

        rb.constraints = RigidbodyConstraints2D.FreezeAll; //Ϊ�����ӳ�ȥ����δ��ײ���κ���ײ��Ľ��ܻ������Ͳ���ȡ�˷�ʽ������ȡ��������������ķ�ʽ
        transform.parent = null;
        isReturning = true;


        // �������� �� ����ȴ
    }

    private void Update()
    {
        if (canRotate)
        {
            //�ǽ����ٶȸ���Unity�������ƶ�Ŀ��ģ���е�x��(�ƶ�ʱ��x�᷽���ϵĺ�ɫ��ͷ)��������
            transform.right = rb.velocity;
        }

        if (isReturning)
        {  //MoveTowards����,(�����￪ʼ���������ƶ�����ʲô�ٶ�),ʹ���������ʱҪ���� Vector2
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
                    isSpinning = false;  //ͣת֮�����
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)  //�� hitCooldown ������ Damage() ���ʱ�� ÿ��һ�� hitCooldown �� Dmage() һ��
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        //��ײ��⣬��������(hit)�Ķ����Ƿ����Enemy��������ȷʵ��������������ô�ö����transform������ӵ�enemyTarget�б���
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

            //����һ������Ŀ��ľ���С�� .1f ��ʱ�򣬸����� target++ Ȼ���䵽��һ��Ŀ�꣬�������
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

    private void OnTriggerEnter2D(Collider2D collision)  //����С��Խ�ҵ�ʱ���õ�����һ����ײ������,Collider2D collision��Unity�����е���ײ�����ͺ���ײ������
    {
        if (isReturning)
        {
            return;    // ��������ڷ���ʱ���ǲ��ᴥ����ײ��������
        }

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);  //���뺯������� Sword û������ Enemy,�ͻ���� Ground ����
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
        if (collision.GetComponent<Enemy>() != null) //�� Sword �� Enemy ��ײ�󴥷���һ˲��¼�������Χ�ڵĵ���
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);  //�������� hit �� colliders�еı���
                                                         //hit ���� Enemy �� transform ���뵽 enemyTarget ��ȥ 
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
            StopWhenSpinning();//��������ʱ��ͣ��ԭ����ת
            return;
        }

        canRotate = false;
        cd.enabled = false;  //�ý�����ײ��ʧЧ

        rb.isKinematic = true;  //��Body Type ��� Kinematic
        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //���������������˶�����
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;  //���ڵ���ʱ����ֹͣ��ת��Ҳ���Ῠ�ڱ�ĸ����ϣ����뵽������ȥ
        }

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;  //������ײ���������Ϊ���ĸ���
    }
}
