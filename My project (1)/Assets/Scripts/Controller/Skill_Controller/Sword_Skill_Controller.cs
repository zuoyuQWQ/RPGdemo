using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Player player;

    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float swordReturnSpeed;

    [Header("反弹剑信息")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("穿刺剑信息")]
    private bool isPierce;
    private int pierceAmount;

    [Header("旋转剑信息")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }

    public void SetupSword(float _power, float _gravityScale, Player _player,float _freezeTimeDuration,float _swordReturnSpeed)
    {
        swordReturnSpeed = _swordReturnSpeed;
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        rb.velocity = new Vector2(_power * player.facingDir, rb.velocity.y);
        rb.gravityScale = _gravityScale;
        if(pierceAmount <= 0 )
            anim.SetBool("Rotation", true);

        Invoke("DestroyMe", 7f);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces,float _bounceSpeed)
    {
        bounceSpeed = _bounceSpeed;
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(bool _isPiece, int _pierceAmount)
    {
        isPierce = _isPiece;
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning,float _maxTravelDistance,float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    private void FixedUpdate()
    {

        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, PlayerManager.instance.player.transform.position, swordReturnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 2)
            {
                PlayerManager.instance.player.CatcTheSword();
            }
        }
        //反弹剑逻辑
        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)//到达最大距离开始旋转
            {
                spinTimer -= Time.deltaTime;
                if (spinTimer < 0)
                {
                    isReturning = true;
                    isPierce = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }

        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                enemyTarget[targetIndex].GetComponent<Enemy>().DamageEffect();

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        collision.GetComponent<Enemy>()?.DamageEffect();
        SetupTargetForBounce(collision);
        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
        enemy.FreezeTimeFor(freezeTimeDuration);

        ItemDataEquipment equipmentAmulet = Inventory.Instance.GetEquipment(EquipmentType.Amulet);

        if (equipmentAmulet != null)
        {
            equipmentAmulet.Effect(enemy.transform);
        }
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 1)
            return;
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
