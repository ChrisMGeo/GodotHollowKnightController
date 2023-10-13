using System;
using Godot;

public partial class Dash : HollowKnightControllerState
{
    private int _dashDir;
    public override void Enter(StateMessage _msg)
    {
        Player.CurrentDashes += 1;
        Player.DashTimer = Player.DashTime;
        if (_msg is HollowKnightControllerStateMessage msg && msg.DashDir != 0)
        {
                _dashDir = msg.DashDir;
        }
        else
        {
            _dashDir = Mathf.IsZeroApprox(Player.DirectionalInput.X) ? Player.FacingDirection : Math.Sign(Player.DirectionalInput.X);
        }

        Player.FacingDirection = _dashDir;
        Player.AnimatedSprite.Play("Dash");
        Player.Velocity = new Vector2(_dashDir * (float)Player.DashSpeed, 0.0f);
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Player.DashTimer <= 0.0)
        {
            if (Player.IsOnFloor())
            {
                if (Mathf.IsZeroApprox(Player.DirectionalInput.X))
                {
                    Player.AnimatedSprite.Play("DashToIdle");
                    StateMachine.TransitionTo("Idle", new HollowKnightControllerStateMessage(){SkipAnimation = true});
                    return;
                }
                else
                {
                    Player.AnimatedSprite.Play("IdleToRun");
                    StateMachine.TransitionTo("Run", new HollowKnightControllerStateMessage(){SkipAnimation = true});
                    return;
                }
            }
            StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage());
            return;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            Player.DashTimer = 0.0;
            StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage(){DoJump = true});
            return;
        }
    }

    public override void Exit()
    {
        Player.Velocity = new Vector2(0.0f, Player.Velocity.Y);
    }
}