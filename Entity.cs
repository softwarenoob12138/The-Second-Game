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
    [SerializeField] protected float groundCheckDistance;  //敌人的脚本也需要墙体和接地检测，所以继承这些
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int konckbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    //System.Action 是一个委托类型，用于封装没有参数且不返回值的方法
    //这种模式常用于事件系统，允许对象之间以 解耦(指减少软件组件之间的直接依赖关系) 的方式通信
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

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower; // 用于给不同的敌人不同的击退效果


    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);


        if (knockbackPower.x > 0 || knockbackPower.y > 0) // 这一行判断语句可以使玩家被攻击后 不会停顿
        {
            rb.velocity = new Vector2((knockbackPower.x + xOffset) * konckbackDir, knockbackPower.y); //* -facingDir是因为击退是要往面向的反方向退
        }

        yield return new WaitForSeconds(knockbackDuration);  //在knockbackDuration时间里会产生被击中时的速度
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()  // 这只是在 Player 里面 设计了一个继承，并不打算用于 Enemy
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

        // 检查 onFlipped 是否已经分配了方法，如果已分配，就调用
        //否则不调用，避免调用空委托时报错
        if (onFlipped != null) 
        { 
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)   //朝左时，给它一个朝右的速度时，就转向
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
            return;  //意味着被击中时会调用isKnocked里的新速度，而不会产生这个函数里的新速度
        }
        rb.velocity = new Vector2(0, 0);
    }


    public  void SetVelocity(float _xVelocity, float _yVelocity)  //这里统一管理Player的x轴方向和y轴方向上的速度
    {
        if(isKnocked)
        {
            return;  
        }
        rb.velocity = new Vector2(_xVelocity, _yVelocity);  //这里用vector2是因为刚体组件用的是2D的   
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    protected virtual void OnDrawGizmos()    //public 就是所有脚本都能用， protected 就是继承者才能用
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
