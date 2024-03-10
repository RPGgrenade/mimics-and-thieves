using Godot;
using System;

public partial class RandomRotater : RoomRandom
{
	[Export] public float RotationRange = 180f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        Randomizer = Randomizer ?? this;
        Randomize();
	}

    public override void Randomize()
    {
		Randomizer.Rotate(Vector3.Up, (float)GD.RandRange(-RotationRange, RotationRange));
    }
}
