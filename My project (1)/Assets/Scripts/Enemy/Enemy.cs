using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("移动属性")]
    public float moveSpeed;
    public float idleTime;
    public float battleSpeed;
    public float battleTime;


    [Header("玩家检测")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected Transform playerCheck;

    protected float PE_Distance;

    [Header("攻击信息")]
    public float attackDistance;
    public float attackCoolDown;
    public float seePlayerDistance;
    public float seePlayerBackDistance;
    [HideInInspector] public float LastTimeAttacked;

    [Header("硬直信息")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject CounterImage;

    private float defaultmoveSpeed;
    private float defaultBattleSpeed;


    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultmoveSpeed = moveSpeed;
        defaultBattleSpeed = battleSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            battleSpeed = 0;
            anim.speed = 0;
           
        }
        else
        {
            moveSpeed = defaultmoveSpeed;
            battleSpeed = defaultBattleSpeed;
            anim.speed = 1;
            
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region 攻击提示及反击相关
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        CounterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        CounterImage.SetActive(false);
    }

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    #endregion

    #region 玩家检测
    public virtual bool IsPlayerDetected() => Physics2D.Raycast(playerCheck.position, Vector2.right*facingDir, playerCheckDistance, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance, playerCheck.position.y));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
    #endregion

    public virtual void AnimationFinishTrigger() =>stateMachine.currentState.AnimationFinishTrigger();

    public override void Die()
    {
        base.Die();
    }

    public override void SlowEntityBY(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        battleSpeed = battleSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultmoveSpeed;
        battleSpeed = defaultBattleSpeed;
    }

}
