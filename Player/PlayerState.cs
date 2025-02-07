using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState   //������ű��������״̬
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float yInput;
    protected float xInput;
    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled; 


    //���캯������Ҫ����������������ʵ���ĳ�ʼ״̬
    //��������Ŷ����˹��캯���Ĳ����б�ÿ�������������ض������ã����ַ�ʽȷ����ÿ��PlayerState�����ܹ����ʵ�������Ҫ����Դ����Ϣ

    //���磬Player _player: ����һ������ΪPlayer�Ĳ��������������ǽ�һ����ҽ�ɫ�����ô��ݸ��´�����PlayerStateʵ��
    //_player��һ������Լ������ʾ����һ��������������ᱻ������ʼ����ĳ�Ա����
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)  //ͨ������������Щ���������Ա��������»��߱�ʾ
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
