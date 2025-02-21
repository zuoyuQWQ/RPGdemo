using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    private Enemy_Skeleton enemy;
    public EnemyDeadState(Enemy _enemyBase, EnemyStateMachine _startMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _startMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        enemy.ZeroVelocity();
    }


}







    //马里奥死亡效果
    //public override void Enter()
    //{
    //    base.Enter();
    //    enemy.anim.SetBool(enemy.lastAnimBoolName, true);
    //    enemy.anim.speed = 0;
    //    enemy.cd.enabled = false;

    //    stateTimer = .1f;
    //}

    //public override void Exit()
    //{
    //    base.Exit();
    //}

    //public override void Update()
    //{
    //    base.Update();
    //    if(stateTimer > 0)
    //    {
    //        rb.velocity = new Vector2(0, 10);
    //    }
    //}

