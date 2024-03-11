using Godot;
using System;

public partial class RandomRotater : RoomRandom
{
	[Export] public float RotationRange = 180f;

    public override void Randomize()
    {
        Randomizer = Randomizer ?? this;
		Randomizer.Rotate(Vector3.Up, (float)GD.RandRange(-RotationRange, RotationRange));
    }
}
