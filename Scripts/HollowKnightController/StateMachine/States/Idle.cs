using System;
using Godot;

public partial class Idle : HollowKnightControllerState
{
    public override void Enter(StateMessage msg)
    {
        Player.CoyoteTimer = Player.CoyoteTime;
        Player.Velocity = Vector2.Zero;
        if (msg is not HollowKnightControllerStateMessage { SkipAnimation: true })
        {
            Player.AnimatedSprite.Play("Idle");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        if (!Player.IsOnFloor())
        {
            StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage());
            return;
        }

        if (Player.EnableJumpBuffer ? Player.JumpBuffer > 0.0 : Input.IsActionJustPressed("jump"))
        {
            StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage() { DoJump = true });
            Player.JumpBuffer = 0.0;
            return;
        }
        if (Input.IsActionPressed("left") || Input.IsActionPressed("right"))
        {
            Player.AnimatedSprite.Play(Math.Sign(Player.DirectionalInput.X) == -Math.Sign(Player.FacingDirection)
                ? "Turn"
                : "IdleToRun");
            StateMachine.TransitionTo("Run", new HollowKnightControllerStateMessage() { SkipAnimation = true });
            return;
        }

        if (Input.IsActionJustPressed("dash") && Player.CurrentDashes < Player.DashLimit)
        {
            StateMachine.TransitionTo("Dash", new HollowKnightControllerStateMessage());
            return;
        }
    }
}