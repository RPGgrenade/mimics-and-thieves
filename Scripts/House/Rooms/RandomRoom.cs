using Godot;
using MEC;
using System;
using System.Collections.Generic;

public partial class RandomRoom : RoomRandom
{
	[Export] public float TimeBetweenSetup = 0.05f;
	[Export] public RoomRandom[] Randomizers;

    private int randomIndex = 0;

    IEnumerator<double> RunRandomization()
    {
        Randomize();
        randomIndex++;
        if (randomIndex < Randomizers.Length)
        {
            yield return Timing.WaitForSeconds(TimeBetweenSetup);
            Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
        }
    }


    public override void _Ready()
	{
        Randomizer = Randomizer ?? this;
        Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
    }

	public override void Randomize()
	{
        GD.Print("Randomizing "+ Randomizers[randomIndex].Name);
        Randomizers[randomIndex].Randomize();
	}
}
