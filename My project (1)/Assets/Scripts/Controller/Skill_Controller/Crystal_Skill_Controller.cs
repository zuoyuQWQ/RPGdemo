using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Player player;
    private Animator anim =>GetComponent<Animator>();
    private CircleCollider2D cd =>GetComponent<CircleCollider2D>();
    private float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;
    private Transform closetEnemy;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool canGrow;
    [SerializeField] private float growSpeed;


    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius,whatIsEnemy);
        if(colliders.Length > 0 )
        {
            closetEnemy = colliders[Random.Range(0, colliders.Length)].transform;
        }
    }

    public void  SetupCrystal(float _crystalDuration,bool _canExplode,bool _canMove,float _moveSpeed,Transform _closetEnemy,Player _player)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closetEnemy = _closetEnemy;
        player = _player;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;
        if(crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if(canMove)
        {
            if(closetEnemy == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position,closetEnemy.position,moveSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position,closetEnemy.position)<1 )
            {
                canMove = false;
                FinishCrystal();
            }
        }

        if(canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(3,3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());

                ItemDataEquipment equipmentAmulet = Inventory.Instance.GetEquipment(EquipmentType.Amulet);

                if(equipmentAmulet != null)
                {
                    equipmentAmulet.Effect(hit.transform);
                }

            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);
}
