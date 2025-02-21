using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


        player.skill.clone.CreatCloneOnDashStart();

        stateTimer = player.dashDurition;

    }

    public override void Exit()
    {
        base.Exit();
        player.skill.clone.CreateCloneOnDashOver();
        player.SetVelocity(0,rb.velocity.y);   
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(player.dashDir * player.dashSpeed, rb.velocity.y);

        if (stateTimer < 0 )
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
