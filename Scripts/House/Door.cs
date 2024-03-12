using Godot;
using System;

public partial class Door : Node3D
{
	[Export] public Node3D DoorObject;
	[Export] public bool IsOpen = false;
	[Export] public float ClosedAngle = 0f;
	[Export] public float OpenAngle = 0f;
	[Export] public float DoorSpeed = 3f;

	[Export] public SoundManager Manager;
	[Export] public AudioStream Sound;

	private float doorAngle = 0f;

	public void Open()
	{
		IsOpen = true;
		Manager.PlayOneShot(Sound, -5f, 1.0f, 1.1f);
		// Play sound
	}

	public void Close()
	{
		IsOpen = false;
        Manager.PlayOneShot(Sound, -5f, 0.8f, 0.9f);
        // Play sound
    }

    public override void _Ready()
    {
		doorAngle = IsOpen ? OpenAngle : ClosedAngle;
        DoorObject.RotationDegrees = new Vector3(DoorObject.RotationDegrees.X, doorAngle, DoorObject.RotationDegrees.Z);
    }

    public override void _PhysicsProcess(double delta)
    {
		doorAngle = Mathf.Lerp(doorAngle, IsOpen ? OpenAngle : ClosedAngle, (float)delta * DoorSpeed);
		DoorObject.RotationDegrees = new Vector3(DoorObject.RotationDegrees.X, doorAngle, DoorObject.RotationDegrees.Z);
    }
}
