 using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    
    public bool isBusy {  get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir {  get; private set; }

    public SkillManager skill {  get; private set; }
    public GameObject sword{  get; private set; }

    public PlayerFX fx { get; private set; }


    //#region 和 #endregion 用来囊括同一个属性，看起来会更整洁
    #region States
    public PlayerStateMachine stateMachine {  get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword {  get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake() //Awake函数就是优先级比Start还高的函数
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");  //这里给idleState已经定义好了变量了，之后调用或者继承就是这个变量
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {

        base.Start();

        fx = GetComponent<PlayerFX>();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);


        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;

    }

    protected override void Update()
    {

        if(Time.timeScale == 0)
        {
            return;  // 时间尺度为 0 时 不接受任何输入
        }

        base.Update();

        stateMachine.currentState.Update();
        
        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) // Alpha1 就是 键盘上的数字 1
        {
            Inventory.instance.UseFlask();
        }
        
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * ( 1 - _slowPercentage);
        jumpForce = jumpForce * ( 1 - _slowPercentage);
        dashSpeed = dashSpeed * ( 1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }


    public IEnumerator BusyFor(float _seconds)   //此处的知识点叫协程，让代码分时执行，让原本可以从上到下完整执行的代码分段 
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);   //将isBusy设置为真，在此处等待一定的秒数，然后再设置为假

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    //箭头函数
    //等价于:
    //    public void AnimationTrigger()
    //    {
    //        stateMachine.currentState.AnimationFinishTrigger();
    //    }

    private void CheckForDashInput()
    {

        if(IsWallDetected())
        {
            return;   //这里的意思就是，碰到墙以后不让用dash
        }

        if(skill.dash.dashUnlocked == false)
        {
            return;
        }

   
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())   //dahsUsageTimer没赋值之前默认值为0
        {
            
            dashDir = Input.GetAxisRaw("Horizontal");  //加入下面这些语句之后就能在当前方向不转向直接朝另一个方向冲刺，用来提高玩家灵活性

            if (dashDir == 0)
            {
               dashDir = facingDir;
            }


            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);

    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }

}
