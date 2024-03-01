using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[Icon("res://addons/StateMachine/IconM.png")]
public partial class StateMachine : Node
{
    [Export(PropertyHint.NodeType)] public State InitialState;

	public Dictionary<string, State> States = new Dictionary<string, State>();
	public State CurrentState;

	public override void _Ready()
	{
		Dictionary<string, State> dict = new();
		foreach (Node state in GetChildren())
		{
			if (state is State)
			{
				State new_state = state as State;
				dict.Add(state.Name.ToString(), new_state);
			}
		}
        States = dict;


		if (InitialState == null)
			InitialState = States.First().Value;
		else
		{
			InitialState.Enter();
			CurrentState = InitialState;
		}
	}

    public override void _Process(double delta)
	{
		if (CurrentState != null)
			CurrentState.Update((float)delta);
	}

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState != null)
            CurrentState.PhysicsUpdate((float)delta);
    }

    public void OnChildTransition(State from_state, string new_state_name)
    {
        if (from_state != CurrentState)
			return;

		State new_state = States[new_state_name];
		if (new_state == null || new_state == from_state) return;

		if (CurrentState != null)
			CurrentState.Exit();
		new_state.Enter();

		CurrentState = new_state;

		GD.Print("From "+ from_state + " to "+ new_state);
	}
}
