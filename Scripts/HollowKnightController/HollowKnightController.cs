using Godot;

public partial class HollowKnightController : CharacterBody2D
{
	public AnimatedSprite2D AnimatedSprite;
	private RayCast2D _leftWall;
	private RayCast2D _rightWall;
	[Export] public Marker2D CameraOffset;

	[ExportGroup("Grounded Movement")] [Export]
	public double GroundedMaxSpeed = 592.0;

	[ExportGroup("Aerial Movement")] [Export]
	public double AerialMaxSpeed = 592.0;
	
	[ExportGroup("Jump Physics")] [Export] public int Jumps = 2;
	public int CurrentJumps = 0;
	[Export] private double _jumpHeight = 360;
	[Export] private double _jumpPeakTime = .51;
	[Export] private double _jumpDescentTime = .5;
	public double JumpGravity;
	public double JumpVelocity;
	public double FallGravity;
	public double Gravity;

	[ExportGroup("Variable Jump Height")] [Export] public bool EnableVariableJumpHeight = true;
	[Export] public double VyMultiplier = .125;

	[ExportGroup("Jump Buffer")] [Export] public bool EnableJumpBuffer = true;
	[Export] public double JumpBufferTime = .1;
	public double JumpBuffer = 0.0;

	[ExportGroup("Coyote Time")] [Export] public bool EnableCoyote = true;
	[Export] public double CoyoteTime = .1;
	public double CoyoteTimer = 0.0;

	[ExportGroup("Fastfalling")] [Export] public bool EnableFastfall = false;
	[Export] public double FastfallingGravity = 1200.0;

	[ExportGroup("Sprite")] [Export] public bool ReverseSprite = false;
	
	[ExportGroup("Wall Slide and Wall Jump")] [Export] public bool EnableWallSlideAndWallJump =  true;
	[Export] public double WallSlideVelocity =  585;
	[Export] public bool DetachFromWallOnNoInput =  false;
	[Export] public double WallJumpAngleInDegrees =  41.6;
	[Export] public double WallJumpSpeed =  1300.0;
	[Export] public double WallJumpTime =  0.1;
	public double WallJumpTimer =  0.0;
	
	[ExportGroup("Dash")]
	[Export] public bool EnableDash =  true;
	[Export] public double DashSpeed =  1200.0;
	[Export] public double DashTime =  0.24;
	[Export] public int DashLimit =  1;
	public double DashTimer =  0.0;
	public int CurrentDashes =  0;
	
	[ExportGroup("Camera Controls")]
	[Export] public double HorizontalCameraBias =  32.0;
	[Export] public double HorizontalAdjustTime =  0.125;
	public double HorizontalCameraMovement;

	public int FacingDirection = 1;

	public Vector2 DirectionalInput => new Vector2(Input.GetActionStrength("right") - Input.GetActionStrength("left"),
		Input.GetActionStrength("down") - Input.GetActionStrength("up"));

	public Vector2 Direction => DirectionalInput.Normalized();
	public bool Jumped => CurrentJumps > 0;
	public int WallSum => (_rightWall.IsColliding()?1:0) - (_leftWall.IsColliding()?1:0);

	public override void _Ready()
	{
		AnimatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite");
		_leftWall = (RayCast2D)GetNode("LeftWall");
		_rightWall = (RayCast2D)GetNode("RightWall");
		JumpGravity = (2.0 * _jumpHeight) / (_jumpPeakTime * _jumpPeakTime);
		JumpVelocity = 1.0 * JumpGravity * _jumpPeakTime;
		FallGravity = (2.0 * _jumpHeight) / (_jumpDescentTime * _jumpDescentTime);
		Gravity = FallGravity;
		HorizontalCameraMovement = HorizontalCameraBias / HorizontalAdjustTime;
	}

	private bool bFD => FacingDirection == -1;

	public override void _Process(double delta)
	{
		Gravity = Velocity.Y >= 0 ? FallGravity : JumpGravity;
		AnimatedSprite.FlipH = ReverseSprite ? bFD : !bFD;
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
		if (Input.IsActionJustPressed("jump"))
		{
			JumpBuffer = JumpBufferTime;
		} else if (JumpBuffer >= 0.0)
		{
			JumpBuffer -= delta;
		}

		if (WallJumpTimer >= 0.0)
		{
			WallJumpTimer -= delta;
		}

		if (DashTimer >= 0.0)
		{
			DashTimer -= delta;
		}

		
		CameraOffset?.Position.MoveToward(new Vector2((float)(HorizontalCameraBias*FacingDirection), CameraOffset.Position.Y), (float)(HorizontalCameraMovement*delta));

		if (IsOnFloor())
		{
			CurrentJumps = 0;
			CurrentDashes = 0;
		}
	}
}
