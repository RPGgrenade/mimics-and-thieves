using Godot;
using System;

public partial class Door : Node3D
{
	[Export] public Node3D DoorObject;
	[Export] public bool IsOpen = false;
	[Export] public float ClosedAngle = 0f;
	[Export] public float OpenAngle = 0f;
	[Export] public float DoorSpeed = 3f;

	private float doorAngle = 0f;

	public void Open()
	{
		IsOpen = true;
		// Play sound
	}

	public void Close()
	{
		IsOpen = false;
		// Play sound
	}

    public override void _Ready()
    {
		doorAngle = IsOpen ? OpenAngle : ClosedAngle;
    }

    public override void _PhysicsProcess(double delta)
    {
		doorAngle = Mathf.Lerp(doorAngle, IsOpen ? OpenAngle : ClosedAngle, (float)delta * DoorSpeed);
		DoorObject.RotationDegrees = new Vector3(DoorObject.RotationDegrees.X, doorAngle, DoorObject.RotationDegrees.Z);
    }
}
