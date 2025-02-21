using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("��ײ���")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("������Ϣ")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;


    #region Compentens
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFx fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    public bool IsBusy { get; private set; }

    #region ��ת
    public int facingDir { get; private set; } = 1;   
    protected  bool facingRight = true;
    #endregion

    public System.Action onFlip;
    protected virtual void Awake()
    {
        fx = GetComponent<EntityFx>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {

    }
    
    public virtual void SlowEntityBY(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    #region ͣ��һ���ִ��
    protected virtual IEnumerator BusyFor(float _seconds)
    {
        IsBusy = true;
        yield return new WaitForSeconds(_seconds);
        IsBusy = false;
    }
    #endregion

    #region ��ײ���
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    #endregion

    #region �˺����
    public virtual void DamageEffect()
    {
       
        StartCoroutine("HitKnockback");
        
    }
    #endregion

    #region ������Ϣ
    protected virtual IEnumerator HitKnockback()
    {

        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -facingDir , knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }
    #endregion
    #region �������
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region ��ת
    public virtual void Flip()
    {
        if (isKnocked)
        {
            return;
        }
        
        
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if(onFlip != null)
            onFlip();

    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion

    #region ˮƽ�ƶ�
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
        {
            return;
        }
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region ֹͣ�ƶ�
    public void ZeroVelocity()
    {
        rb.velocity = new Vector2(0,0);
    }
    #endregion




    public virtual void Die()
    {

    }

    //ʬ����ʧЧ��,ռλ����
    public virtual void BodyDisappear()
    {
        Destroy(gameObject);
    }
}
