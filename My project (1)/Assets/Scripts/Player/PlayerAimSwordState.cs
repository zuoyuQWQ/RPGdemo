using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.3f);
    }

    public override void Update()
    {
        base.Update();
        player.ZeroVelocity();
        if (Input.GetKeyUp(KeyCode.K))
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
