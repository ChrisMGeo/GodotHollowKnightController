using System;
using Godot;

public partial class StateMachine : Node
{
	[Signal]
	public delegate void TransitionedEventHandler(StringName stateName);

	[Export]
	public State InitialState = null;
	
	private State _state;
	
	public override async void _Ready()
	{
		_state = InitialState;
		await ToSignal(Owner, "ready");
		var children = GetChildren();
		foreach (var child in children)
		{
			if (child is State childState)
			{
				childState.StateMachine = this;
			}
		}
		_state.Enter(new State.StateMessage());
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		_state.HandleInput(inputEvent);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_state.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_state.PhysicsUpdate(delta);
	}

	public void TransitionTo(string targetStateName, State.StateMessage msg = null)
	{
		msg ??= new State.StateMessage();
		if (!HasNode(targetStateName))
		{
			return;
		}
		_state.Exit();
		_state = (State)GetNode(targetStateName);
		_state.Enter(msg);
		EmitSignal(SignalName.Transitioned, _state.Name);
	}
}
