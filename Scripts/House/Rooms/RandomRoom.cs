using Godot;
using MEC;
using System;
using System.Collections.Generic;

public partial class RandomRoom : RoomRandom
{
    [Export] public float TimeForVisibility = 0.05f;
    [Export] public float VisibilityDistance = 20f;
    [Export] public Door ExitDoor;
    [Export] public AreaExit Exit;
    [ExportCategory("Randomizing")]
    [Export] public float TimeBetweenSetup = 0.05f;
    [Export] public float TimeBetweenSpawning = 0.5f;
    [Export] public float TimeBeforeStopping = 2.5f;
    //[Export] public RoomRandom[] Randomizers;
    [Export] public RandomRotater[] Rotaters;
    [Export] public RandomLighter[] Lighters;
    [Export] public RandomDespawn[] Furniture;
    [Export] public RandomMimic[] Mimics;
    [Export] public RandomLoot[] Loot;

    private RoomRandom[] randomizers;
    private int randomIndex = 0;
    private float randomizationTime = 0f;
    public int listcount = 0;

    //private Node3D camera;
    //private float distanceFromCamera = float.MaxValue;

    IEnumerator<double> RunRandomization()
    {
        Randomize();
        randomIndex++;
        if (randomIndex < randomizers.Length)
        {
            yield return Timing.WaitForSeconds(randomizationTime);
            Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
        }
        else
        {
            randomIndex = 0;
            listcount++;
            switch (listcount)
            {
                case 1: randomizers = Lighters; randomizationTime = TimeBetweenSetup; break;
                case 2: randomizers = Furniture; randomizationTime = TimeBetweenSetup; break;
                case 3: randomizers = Mimics; randomizationTime = TimeBetweenSpawning; break;
                case 4: randomizers = Loot; randomizationTime = TimeBetweenSpawning; break;
                default: break;
            }
            if (listcount < 5)
            {
                yield return Timing.WaitForSeconds(randomizationTime);
                Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
            }
        }
        if (listcount == 5)
        {
            yield return Timing.WaitForSeconds(TimeBeforeStopping); // To give objects time to reach the floor and not activate mimics
            listcount++;
            CarryData.Instance.Loaded = true;
        }
    }


    public override void _Ready()
	{
        Randomizer = Randomizer ?? this;
        if(ExitDoor != null)
            Exit.ExitDoor = ExitDoor;
        randomizers = Rotaters;
        randomizationTime = TimeBetweenSetup;
        //camera = GetViewport().GetCamera3D();
        Timing.RunCoroutine(RunRandomization().CancelWith(this), Segment.Process, "Random");
        //Timing.RunCoroutine(CheckVisibilty().CancelWith(this), Segment.Process, "Visible");
    }

	public override void Randomize()
	{
        //GD.Print("Randomizing "+ randomizers[randomIndex].Name);
        randomizers[randomIndex].Randomize();
	}
}
