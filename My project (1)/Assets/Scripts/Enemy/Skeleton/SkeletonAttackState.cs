using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _startMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _startMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.LastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(0, rb.velocity.y);
        if(triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
