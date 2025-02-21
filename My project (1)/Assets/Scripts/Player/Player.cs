using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Player : Entity
{
    [Header("基础攻击信息")]
    public Vector2[] attackMovement;
    public float counterAttackDuration =.2f;

    [Header("飞剑信息")]
    public float swordReturnForce =3.5f;
    public GameObject sword {  get; set; }

    [Header("基本属性")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("冲刺信息")]
    public float dashSpeed = 15f;
    public float dashDurition = 0.3f;
    public float dashDir { get; private set; }


    private float defaultSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeed;
    public SkillManager skill {  get; private set; }


    #region States
    public PlayerStateMachine stateMachine {  get; private set; }

    public PlayerIdleState idleState { get; private set; }

    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }

    public PlayerAirState airState { get; private set; }  
    
    public PlayerDashState dashState { get; private set; }

    public PlayerWallSlideState wallSlideState { get; private set; }
    
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine,"Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");

        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
    }

    protected override void Start()
    {
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.U))
        {
            skill.crystal.CanUseSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.Instance.UseFlask();
        }
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatcTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    private void CheckForDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsGroundDetected() && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = facingDir;
            stateMachine.ChangeState(dashState);
        }
    }

    public void AnimationTrigger() =>stateMachine.currentState.AnimationFinishTrigger();


    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }


    public override void SlowEntityBY(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1-_slowPercentage);
        jumpForce = jumpForce * (1-_slowPercentage);
        dashSpeed = dashSpeed * (1-_slowPercentage);
        anim.speed = anim.speed * (1-_slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultSpeed;
        dashSpeed = defaultDashSpeed;
        jumpForce = defaultJumpForce;
    }
}
