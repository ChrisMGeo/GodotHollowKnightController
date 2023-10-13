using System;
using Godot;

public partial class WallSlide : HollowKnightControllerState
{
    private int _wallDirection;
    public override void Enter(StateMessage _msg)
    {
        Player.CurrentJumps = 1;
        Player.CurrentDashes = 0;
        Player.WallJumpTimer = 0.0;
        if (_msg is HollowKnightControllerStateMessage msg && msg.WallDirection != 0)
        { _wallDirection = msg.WallDirection;}
        Player.AnimatedSprite.Play("WallSlide");
        Player.Velocity = new Vector2(0f, (float)Player.WallSlideVelocity);
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Player.IsOnFloor())
        {
            Player.AnimatedSprite.Play("Land");
            StateMachine.TransitionTo("Idle", new HollowKnightControllerStateMessage(){SkipAnimation = true});
            return;
        }

        if (Player.WallSum == 0 || (Player.DetachFromWallOnNoInput && Player.DirectionalInput.X == 0.0) ||
            Math.Sign(Player.DirectionalInput.X) == -_wallDirection)
        {
            StateMachine.TransitionTo("Air");
            return;
        }

        if (Player.EnableJumpBuffer ? Player.JumpBuffer > 0.0 : Input.IsActionJustPressed("jump"))
        {
            StateMachine.TransitionTo("WallJump");
            return;
        }

        if (Input.IsActionJustPressed("dash") && Player.CurrentDashes < Player.DashLimit)
        {
            StateMachine.TransitionTo("Dash");
            return;
        }
    }

    public override void Exit()
    {
        Player.FacingDirection = -_wallDirection;
    }
}