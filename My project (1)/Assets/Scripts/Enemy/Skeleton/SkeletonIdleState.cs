using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _startMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _startMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0,rb.velocity.y);

        stateTimer = enemy.idleTime;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {       
        if (enemy.IsWallDetetected() || !enemy.IsGroundDetected())
        {
            if(stateTimer < 0)
            {
                enemy.Flip();
                stateMachine.ChangeState(enemy.moveState);

            }
        }
        else if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
        base.Update();
    }
}
