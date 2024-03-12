using Godot;
using MEC;
using System;
using System.Collections.Generic;

public partial class RandomRoom : RoomRandom
{
    [Export] public Door ExitDoor;
    [Export] public AreaExit Exit;
    [ExportCategory("Randomizing")]
	[Export] public float TimeBetweenSetup = 0.05f;
	//[Export] public RoomRandom[] Randomizers;
    [Export] public RandomRotater[] Rotaters;
    [Export] public RandomLighter[] Lighters;
    [Export] public RandomDespawn[] Furniture;
    [Export] public RandomMimic[] Mimics;
    [Export] public RandomLoot[] Loot;

    private RoomRandom[] randomizers;
    private int randomIndex = 0;
    private int listcount = 0;

    IEnumerator<double> RunRandomization()
    {
        Randomize();
        randomIndex++;
        if (randomIndex < randomizers.Length)
        {
            yield return Timing.WaitForSeconds(TimeBetweenSetup);
            Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
        }
        else
        {
            randomIndex = 0;
            listcount++;
            switch (listcount)
            {
                case 1: randomizers = Lighters; break;
                case 2: randomizers = Furniture; break;
                case 3: randomizers = Mimics; break;
                case 4: randomizers = Loot; break;
                default: break;
            }
            if(listcount < 5)
            {
                yield return Timing.WaitForSeconds(TimeBetweenSetup);
                Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
            }
        }
    }


    public override void _Ready()
	{
        Randomizer = Randomizer ?? this;
        if(ExitDoor != null)
            Exit.ExitDoor = ExitDoor;
        randomizers = Rotaters;
        Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
    }

	public override void Randomize()
	{
        GD.Print("Randomizing "+ randomizers[randomIndex].Name);
        randomizers[randomIndex].Randomize();
	}
}
