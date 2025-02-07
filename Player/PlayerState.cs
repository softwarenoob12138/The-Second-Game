using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState   //用这个脚本构建玩家状态
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float yInput;
    protected float xInput;
    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled; 


    //构造函数的主要任务就是设置类的新实例的初始状态
    //这里的括号定义了构造函数的参数列表，每个参数都有其特定的作用，这种方式确保了每个PlayerState对象都能够访问到它所需要的资源和信息

    //例如，Player _player: 这是一个类型为Player的参数，它的作用是将一个玩家角色的引用传递给新创建的PlayerState实例
    //_player是一个命名约定，表示这是一个输入参数，它会被用来初始化类的成员变量
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)  //通过函数传入这些变量，所以变量名用下划线表示
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical"); 

        player.anim.SetFloat("yVelocity", rb.velocity.y);

    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
