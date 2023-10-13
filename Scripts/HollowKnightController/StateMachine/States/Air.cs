using Godot;

public enum AirState
{
    Fall,
    Jump
}

public partial class Air : HollowKnightControllerState
{
    private AirState _airState=AirState.Fall;
    private bool _stoppedJump;

    public override void Enter(StateMessage msg)
    {
	    _stoppedJump = false;
	    if (msg is HollowKnightControllerStateMessage { DoJump: true } && Player.CurrentJumps < Player.Jumps)
	    {
		    Player.CurrentJumps += 1;
		    _airState = AirState.Jump;
		    Player.Velocity = new Vector2(Player.Velocity.X, -(float)Player.JumpVelocity);
		    switch (Player.CurrentJumps)
		    {
			    case 2:
				    Player.AnimatedSprite.Play("DoubleJump");
				    break;
			    default:
				    Player.AnimatedSprite.Play("Jump");
				    break;
		    }
		    Player.AnimatedSprite.Frame = 0;
	    }
    }

    public override void PhysicsUpdate(double delta)
    {
	    if (Player.DirectionalInput.X != 0.0 &&
	        Mathf.Sign(Player.DirectionalInput.X) != Player.FacingDirection)
	    {
		    Player.FacingDirection = Mathf.Sign(Player.DirectionalInput.X);
	    }

	    if (Player.Velocity.Y >= 0)
	    {
		    if (_airState != AirState.Fall)
		    {
			    _airState = AirState.Fall;
			    if (Player.CurrentJumps == 0)
			    {
				    Player.CurrentJumps = 1;
			    }
		    }

		    if (Player.AnimatedSprite.Animation != "Fall" && Player.AnimatedSprite.Animation != "Jump")
		    {
			    if (Player.CurrentJumps != 2)
			    {
				    Player.AnimatedSprite.Play("Jump");
				    Player.AnimatedSprite.Frame = 4;
			    }
			    else
			    {
				    if (Player.AnimatedSprite.Animation != "DoubleJump")
				    {
					    Player.AnimatedSprite.Play("Fall");
				    }
			    }
		    }
	    }

	    if (Player.IsOnFloor())
	    {
		    if (Mathf.IsZeroApprox(Player.DirectionalInput.X))
		    {
			    Player.AnimatedSprite.Play("Land");
			    StateMachine.TransitionTo("Idle", new HollowKnightControllerStateMessage(){SkipAnimation = true});
			    return;
		    }
		    else
		    {
			    StateMachine.TransitionTo("Run");
			    return;
		    }
	    }

	    if ((Player.EnableJumpBuffer ? Player.JumpBuffer > 0.0 : Input.IsActionJustPressed("jump")) &&
	        Player.CoyoteTimer > 0.0 && !Player.Jumped && Player.EnableCoyote)
	    {
		    StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage(){DoJump = true});
		    return;
	    }

	    if (Input.IsActionJustPressed("dash") && Player.CurrentDashes < Player.DashLimit)
	    {
		    StateMachine.TransitionTo("Dash");
		    return;
	    }

	    if (Input.IsActionJustPressed("jump"))
	    {
		    StateMachine.TransitionTo("Air", new HollowKnightControllerStateMessage(){DoJump = true});
		    return;
	    }

	    if (Player.Velocity.Y < 0.0 && !Input.IsActionPressed("jump") && !_stoppedJump)
	    {
		    if (Player.EnableVariableJumpHeight)
		    {
			    Player.Velocity = new Vector2(Player.Velocity.X, (float)(Player.Velocity.Y * Player.VyMultiplier));
		    }

		    Player.CoyoteTimer = 0.0;
		    _stoppedJump = true;
	    }

	    if (Player.CoyoteTimer > 0.0)
	    {
		    Player.CoyoteTimer -= delta;
	    }
	    else
	    {
		    if (Player.CurrentJumps == 0)
		    {
			    Player.CurrentJumps = 1;
		    }
	    }

	    var finalGravity = Player.Gravity;
	    if (_airState == AirState.Fall && Player.DirectionalInput.Y > 0.0 && Player.EnableFastfall)
	    {
		    finalGravity += Player.FastfallingGravity;
	    }

	    if (Player.WallSum != 0 && Player.WallSum == Mathf.Sign(Player.DirectionalInput.X) && _airState == AirState.Fall)
	    {
		    StateMachine.TransitionTo("WallSlide", new HollowKnightControllerStateMessage(){WallDirection = Player.WallSum});
		    return;
	    }

	    Player.Velocity = new Vector2((float)(Mathf.Sign(Player.DirectionalInput.X) * Player.AerialMaxSpeed),
		    (float)(Player.Velocity.Y + finalGravity * delta));
    }
}