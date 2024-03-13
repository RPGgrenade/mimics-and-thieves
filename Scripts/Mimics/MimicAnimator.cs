using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MimicAnimator : Node
{
	[Export] public bool Active = false;
    [Export] public string FightMusic = "Caught Music";
    [Export] public string RoomMusic = "Room Music";
    [Export] public float ActivityDistanceFromCamera = 22f;

	[ExportCategory("Part References")]
	[Export] public float PartSpeed = 2.5f;
	[Export] public Node3D[] BodyParts;
	[Export] public Node3D[] Eyes;
    [Export] public Node3D Mouth;
    [Export] public Node3D Tongue;

	[ExportCategory("Part Properties")]
	[ExportGroup("Eyes")]
	[Export] public float EyeChance = 0.8f; // Chances each eye will be visible
	[Export] public float LookatSpeed = 1.5f;
	[Export] public Vector2 BlinkTime = new Vector2(0.4f, 3f); // Will be randomized for every eye
	[ExportGroup("Mouth")]
	[Export] public bool UpDown = true;
	[Export] public float ChompSpeed = 4f;
    [Export] public float InactiveAngle = 0f;
    [Export] public float ClosedAngle = 0f;
    [Export] public float OpenAngle = -40f;
	[Export] public Vector2 ChompTime = new Vector2(3.4f, 5.5f);
	[ExportGroup("Tongue")]
	[Export] public float TongueSpeed = 2.3f;
    [Export] public float TongueAngle = 30f;
    [ExportCategory("Sounds")]
    [Export] public SoundManager Manager;

    private List<Vector3> partOriginalSizes = new List<Vector3>();

	private float blinkTime = 0f;
	private float chompTime = 0f;
	private float tongueTime = 0f;

	private float mouthAngle = 0f;

	private bool needsUpdating = false;

	private Vector3 firstTongueRotation = Vector3.Zero;

	private Node3D camera;
	private float distanceFromCamera = float.MaxValue;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		foreach (Node3D part in BodyParts)
		{
			partOriginalSizes.Add(part.Scale);
			part.Scale = Vector3.Zero;
		}
		camera = GetViewport().GetCamera3D();
		firstTongueRotation = Tongue.RotationDegrees;
		mouthAngle = ClosedAngle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int index = 0;
        if (camera != null && IsInstanceValid(camera))
            distanceFromCamera = Mouth.GlobalPosition.DistanceTo(camera.GlobalPosition);
		foreach (Node3D part in BodyParts)
		{
			part.Scale = part.Scale.Lerp(Active ? partOriginalSizes[index] : Vector3.Zero, (float)delta * PartSpeed);
			index++;
        }

		if (distanceFromCamera <= ActivityDistanceFromCamera)
		{
			if (Active)
			{
				tongueTime += (float)delta;
				Tongue.RotateX(Mathf.DegToRad(Mathf.Cos(tongueTime * TongueSpeed) * TongueAngle * (float)delta));
			}

			chompTime -= (float)delta;
			if (chompTime <= 0f || !Active)
			{
				mouthAngle = Mathf.Lerp(mouthAngle, Active ? ClosedAngle : InactiveAngle, (float)delta * ChompSpeed);
				if (UpDown)
					Mouth.RotationDegrees = new Vector3(mouthAngle, Mouth.RotationDegrees.Y, Mouth.RotationDegrees.Z);
				else
					Mouth.RotationDegrees = new Vector3(Mouth.RotationDegrees.X, mouthAngle, Mouth.RotationDegrees.Z);
				GD.Print("Mouth rot: " + Mouth.RotationDegrees);
				if (Mathf.Abs(mouthAngle - ClosedAngle) <= 1f)
				{
					chompTime = (float)GD.RandRange(ChompTime.X, ChompTime.Y);
					if(Active)
						Manager.PlayRandom("Growl", volume: -10f);
				}
			}
			if (chompTime > 0f && Active)
			{
				mouthAngle = Mathf.Lerp(mouthAngle, OpenAngle, (float)delta * ChompSpeed);
                if (UpDown)
                    Mouth.RotationDegrees = new Vector3(mouthAngle, Mouth.RotationDegrees.Y, Mouth.RotationDegrees.Z);
                else
                    Mouth.RotationDegrees = new Vector3(Mouth.RotationDegrees.X, mouthAngle, Mouth.RotationDegrees.Z);
                GD.Print("Mouth rot: " + Mouth.RotationDegrees);
            }
		}
    }

    public void Activate()
    {
		if (distanceFromCamera <= ActivityDistanceFromCamera)
		{
			Active = true;
			RandomizeEyes();
			if (FightMusic != "")
				MusicHandler.Instance.SetOnlyGroupActive(FightMusic, volSpeed : 1f);
		}
    }

    public void Deactivate()
    {
		if (distanceFromCamera <= ActivityDistanceFromCamera)
		{
			Active = false;
			if (FightMusic != "")
				MusicHandler.Instance.SetOnlyGroupActive(RoomMusic, volSpeed: 0.5f);
		}
    }

    private void RandomizeEyes()
	{
		foreach (Node3D eye in Eyes)
		{
			float chance = (float)GD.RandRange(0, EyeChance);
			eye.Scale = chance <= 0.1f ? Vector3.Zero : Vector3.One;
		}
	}
}
