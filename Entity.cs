using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected Vector2 knockbackOffset;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;


    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;  //���˵Ľű�Ҳ��Ҫǽ��ͽӵؼ�⣬���Լ̳���Щ
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int konckbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    //System.Action ��һ��ί�����ͣ����ڷ�װû�в����Ҳ�����ֵ�ķ���
    //����ģʽ�������¼�ϵͳ���������֮���� ����(ָ����������֮���ֱ��������ϵ) �ķ�ʽͨ��
    public System.Action onFlipped; 

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        
    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if(_damageDirection.position.x > transform.position.x)
        {
            konckbackDir = -1;
        }
        else if(_damageDirection.position.x < transform.position.x)
        {
            konckbackDir = 1;
        }
    }

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower; // ���ڸ���ͬ�ĵ��˲�ͬ�Ļ���Ч��


    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);


        if (knockbackPower.x > 0 || knockbackPower.y > 0) // ��һ���ж�������ʹ��ұ������� ����ͣ��
        {
            rb.velocity = new Vector2((knockbackPower.x + xOffset) * konckbackDir, knockbackPower.y); //* -facingDir����Ϊ������Ҫ������ķ�������
        }

        yield return new WaitForSeconds(knockbackDuration);  //��knockbackDurationʱ��������������ʱ���ٶ�
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()  // ��ֻ���� Player ���� �����һ���̳У������������� Enemy
    {

    }

    #region Flip
    public  virtual void Flip()
    {
        if (isKnocked)
        {
            return;
        }

        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        // ��� onFlipped �Ƿ��Ѿ������˷���������ѷ��䣬�͵���
        //���򲻵��ã�������ÿ�ί��ʱ����
        if (onFlipped != null) 
        { 
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)   //����ʱ������һ�����ҵ��ٶ�ʱ����ת��
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }

    public virtual void SetupDefaultFacingDir(int _direction)
    {
        facingDir = _direction;

        if(facingDir == -1)
        {
            facingRight = false; 
        }
    }

    #endregion

    #region Velocity
    public void SetZeroVelocity()
    {
        if(isKnocked)
        {
            return;  //��ζ�ű�����ʱ�����isKnocked������ٶȣ�����������������������ٶ�
        }
        rb.velocity = new Vector2(0, 0);
    }


    public  void SetVelocity(float _xVelocity, float _yVelocity)  //����ͳһ����Player��x�᷽���y�᷽���ϵ��ٶ�
    {
        if(isKnocked)
        {
            return;  
        }
        rb.velocity = new Vector2(_xVelocity, _yVelocity);  //������vector2����Ϊ��������õ���2D��   
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    protected virtual void OnDrawGizmos()    //public �������нű������ã� protected ���Ǽ̳��߲�����
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    public virtual void Die()
    {
        
    }

}
