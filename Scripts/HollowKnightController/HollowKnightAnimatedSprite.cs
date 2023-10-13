using Godot;

public partial class HollowKnightAnimatedSprite : AnimatedSprite2D
{
	private void OnAnimationFinished()
	{
		switch (Animation)
		{
			case "IdleToRun":
				Play("Run");
				break;
			case "Turn":
				Play("IdleToRun");
				break;
			case "RunToIdle":
				Play("Idle");
				break;
			case "Jump":
				Play("Fall");
				break;
			case "DoubleJump":
				Play("Fall");
				break;
			case "Land":
				Play("Idle");
				break;
			case "DashToIdle":
				Play("Idle");
				break;
			case "Dash":
				Play("Fall");
				break;
		}
	}
}
