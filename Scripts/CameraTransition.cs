using Godot;
using System;

public partial class CameraTransition : Camera3D
{
	[Export] public Marker3D FinalPosition;
    [Export] public bool Transition = false;
    [Export] public float TransitionSpeed = 1.5f;

    public override void _PhysicsProcess(double delta)
	{
		if(Transition)
			GlobalPosition = GlobalPosition.Lerp(FinalPosition.GlobalPosition, (float)delta * TransitionSpeed);
	}
}
