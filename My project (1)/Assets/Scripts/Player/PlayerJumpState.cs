using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(rb.velocity.x,rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * xInput * 0.8f, player.rb.velocity.y);
        }

        if (rb.velocity.y <0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
