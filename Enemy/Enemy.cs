using System.Collections;
using UnityEngine;


// ���� Unity �����е�һ�����Ա�ǩ�������ڱ�д�ű�ʱ�����ϣ���ýű�����Զ���鲢ȷ��ĳ���ض�������Ѿ�����ӵ���Ϸ�����ϣ�����ʹ�ø�����
// ������˵��Unity ���Զ�������Ϸ�����Ƿ��Ѿ�������һ�� Rigidbody2D ��������û�У�Unity �����Զ�Ϊ����Ϸ�������һ�� Rigidbody2D ���
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;


    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float agroDistance = 2;
    public float attackDistance;
    public float attackCooldown;
    public float minAttackCooldown;
    public float maxAttackCooldown; // ������ƿ����� 
    [HideInInspector] public float lastTimeAttacked;   //ʹ֮�����ڼ������
    public EnemyStateMachine stateMachine { get; private set; }
    public EntityFX fx { get; private set; }
    private Player player;
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponent<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }


    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds) //��Э��������ʵ��Ӳֵ��ʱ��
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);  //���������ʹ counterImage ���ڻ�Ծ״̬
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);  //ʹ֮�ǻ�Ծ
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y)); // + �ű�ʾ���ұ߻���
    }

}
