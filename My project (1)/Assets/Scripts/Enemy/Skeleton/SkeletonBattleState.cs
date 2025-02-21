using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Enemy_Skeleton enemy;
    private Transform player;
    private int moveDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _startMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _startMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;

    }
    public override void Update()
    {
        base.Update();

        if(enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (Mathf.Abs(enemy.transform.position.x - player.transform.position.x) < enemy.attackDistance)
            {
                if(CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
        }
        else
        {
            if(stateTimer < 0 || Vector2.Distance(player.transform.position,enemy.transform.position) > enemy.seePlayerDistance)
            {
                stateMachine.ChangeState(enemy.idleState);   
            }
        }

        if(player.transform.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if(player.transform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }
            enemy.SetVelocity(enemy.battleSpeed * moveDir, rb.velocity.y);

        if (!enemy.IsGroundDetected() || enemy.IsWallDetetected())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if(Time.time >= enemy.LastTimeAttacked + enemy.attackCoolDown)
        {
            return true;
        }
        
        return false;
    }
}
