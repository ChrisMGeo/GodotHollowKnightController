using System;
using Godot;

public partial class WallJump : HollowKnightControllerState
{
    public override void Enter(StateMessage msg)
    {
        Player.WallJumpTimer = Player.WallJumpTime;
        Player.AnimatedSprite.Play("WallJump");
        Player.Velocity = new Vector2((float)(Math.Sign(Player.FacingDirection) * Player.WallJumpSpeed *
                                              Math.Cos(Player.WallJumpAngleInDegrees * Math.PI / 180)),
            (float)(-Player.WallJumpSpeed * Math.Sin((float)Player.WallJumpAngleInDegrees * Math.PI / 180)));
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Player.WallJumpTimer <= 0.0 || !Input.IsActionPressed("jump"))
        {
            StateMachine.TransitionTo("Air");
            return;
        }
    }
}