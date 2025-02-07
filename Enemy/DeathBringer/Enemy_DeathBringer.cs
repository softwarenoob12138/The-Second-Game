using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    #region State
    public DeathBringerIdleState idleState { get; private set; }
    public DeathBringerTeleportState teleportState { get; private set; }
    public DeathBringerBattleState battleState { get; private set; }
    public DeathBringerAttackState attackState { get; private set; }
    public DeathBringerDeadState deadState { get; private set; }
    public DeathBringerSpellCastState spellCastState { get; private set; }
    #endregion
    public bool bossFightBegun;

    [Header("Spell cast details")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private Vector2 spellOffset;

    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    public float defultChanceToTeleport = 25;

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        idleState = new DeathBringerIdleState(this, stateMachine, "Idle", this);
        teleportState = new DeathBringerTeleportState(this, stateMachine, "Teleport", this);
        battleState = new DeathBringerBattleState(this, stateMachine, "Move", this);  //看见玩家就朝玩家走去
        attackState = new DeathBringerAttackState(this, stateMachine, "Attack", this);
        deadState = new DeathBringerDeadState(this, stateMachine, "Idle", this);
        spellCastState = new DeathBringerSpellCastState(this, stateMachine, "SpellCast", this);

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if(player.rb.velocity.x != 0)
        {
            xOffset = player.facingDir * spellOffset.x;
        }

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellOffset.y);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpell_Controller>().SetupSpell(stats);
    }

    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);

        transform.position = new Vector3(x, y);

        // 对象会在检测到地面时 自动调整位置，使其底部保持在地面之上一定高度
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

        if (!GroundBelow() || SomethingIsAround())
        {
            FindPosition();
        }
    }

    //  GroundBelow 方法会返回一个包含相交信息的 RaycastHit2D 结构体,即从 当前 position 到 whatIsGround 的距离 
    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

    // BoxCast 在角色周围创建了一个矩形检测区域，并检测这个区域内是否有物体
    // surroundingCheckSize 参数是检测框的大小，是一个 Vector2 类型的变量，定义 宽高
    // 这里的 数字 0 是力度，因为这里不需要移动检测框，所以力度设为 0
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));

        //  Gizmos.DrawWireCube 用于绘制一个与 SomethingIsAround 方法中的检测区域相匹配的线框立方体
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defultChanceToTeleport;
            return true;
        }

        return false;
    }

    public bool CanDoSpellCast()
    {

        if (Time.time >= lastTimeCast + spellStateCooldown)
        {
            return true;
        }

        return false;

    }

}
