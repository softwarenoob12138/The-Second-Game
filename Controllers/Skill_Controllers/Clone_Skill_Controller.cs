using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;  //ͼ����ʧ�ٶ�
    
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

    private void Update()  //��¡�屻��������֮�󣬸�cloneTimer��ֵ�Ժ�ʼ��ʱ������ʧ
    {
        cloneTimer -= Time.deltaTime;  

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if(sr.color.a <= 0 )
            {
                Destroy(gameObject);  //ͼ����ȫ��ʧ֮��ɾ�������¡
            }
        }
    }
    public void SetupClone(Transform _newTransform, float _cloneDuration , bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        if(_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));  //������� 1 2 3������֮һ
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
        cloneTimer = -.1f;  //�ڹ�����������֮�����ϸ�ֵ����0���£����乥����������ʼ��ʧ
    }

    private void AttackTrigger()
    {
        //Phisics2D ��OverlapCircleAll���� ��ȡ���������ֱ���һ�����һ���뾶��Ȼ�󹹳ɵ�һ��Բ�������Բ�Ĵ�С��Ϊ��ײ���Ĵ�С
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        //��������var�������������ײ��֮�У��͵���Damage����
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //�����Ǽ���Ƿ���Enemy¼���������ײ��
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>()); ר��Ϊ��¡����˺�����һ������

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier); 

                if(player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                    {
                        weaponData.Effect(hit.transform); // // ʹ��¡����Դ������װ����������Ч��
                    }
                }

                if(canDuplicateClone)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        //ÿһ��Clone����һ��Clone_Skill_Controller,Ĭ�� facingDir Ϊ 1
                        //����������˾ͻᷢ�֣�ÿһ���µ�Cloneʵ����ֻ�ᴴ��һ��facingDir = -1�ĸ�����
                        //�������ڹ���֮���ٽ����ж��Ƿ���һ���µ� Ĭ�� facingDir Ϊ 1��Clone
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()  //����ʹ��¡���������Ŀ��
    {
        
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)  //�����¡������ĵ��˵��ұ߾ͻ�ת�򣬷���ת��������ʵ���˿�¡�������������ĵ���
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
