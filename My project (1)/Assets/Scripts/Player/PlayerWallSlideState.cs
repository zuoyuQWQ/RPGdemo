using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
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

        if(player.IsWallDetetected())
        {
            if (yInput < 0)
            {
                player.SetVelocity(player.moveSpeed * xInput, rb.velocity.y);
            }
            else
            {
                player.SetVelocity(player.moveSpeed * xInput, rb.velocity.y * 0.9f);
            }
        }

        else if(!player.IsWallDetetected())
        {
            stateMachine.ChangeState(player.airState);
        }


        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
