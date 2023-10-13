using Godot;
using System;

public partial class State : Node
{
	public StateMachine StateMachine { get; set; }
	public virtual void HandleInput(InputEvent inputEvent) {}
	public virtual void Update(double delta) {}
	public virtual void PhysicsUpdate(double delta) {}

	public class StateMessage
	{
	}
	public virtual void Enter(StateMessage msg) {}
	public virtual void Exit() {}
}