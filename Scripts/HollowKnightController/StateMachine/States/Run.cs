using System;
using Godot;

public partial class Run : HollowKnightControllerState
{
    public override void Enter(StateMessage msg)
    {
        if (msg is not HollowKnightControllerStateMessage { SkipAnimation: true })
        {
            Player.AnimatedSprite.Play("Run");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Player.EnableJumpBuffer ? Player.JumpBuffer > 0.0 : Input.IsActionJustPressed("jump"))
        {
            StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage(){DoJump = true});
            Player.JumpBuffer = 0.0;
            return;
        }

        if (Mathf.IsZeroApprox(Player.DirectionalInput.X))
        {
            Player.AnimatedSprite.Play("RunToIdle");
            StateMachine.TransitionTo("Idle", new HollowKnightControllerStateMessage(){SkipAnimation = true});
            return;
        }

        if (!Player.IsOnFloor())
        {
            StateMachine.TransitionTo("Air");
            return;
        }

        if (Input.IsActionJustPressed("dash") && Player.CurrentDashes < Player.DashLimit)
        {
            StateMachine.TransitionTo("Dash");
            return;
        }

        if (Player.DirectionalInput.X!=0)
        {
            Player.FacingDirection = Math.Sign(Player.DirectionalInput.X);
        }

        Player.Velocity = new Vector2((float)Player.GroundedMaxSpeed * Math.Sign(Player.DirectionalInput.X), Player.Velocity.Y);
    }
}
