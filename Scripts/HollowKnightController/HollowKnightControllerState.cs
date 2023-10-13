public class HollowKnightControllerStateMessage : State.StateMessage
{
	public bool SkipAnimation { get; set; } = false;
	public bool DoJump { get; set; } = false;
	public int WallDirection { get; set; } = 0;
	public int DashDir { get; set; } = 0;
}

public partial class HollowKnightControllerState : State
{
	protected HollowKnightController Player { get; set; }

	public override async void _Ready()
	{
		await ToSignal(Owner, "ready");
		Player = (HollowKnightController)Owner;
	}
}
