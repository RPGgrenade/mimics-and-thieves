using Godot;
using System;

public partial class MimicAnimator : Node
{
	[Export] public bool Active = false;

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
    [Export] public float ClosedAngle = 0f;
    [Export] public float OpenAngle = -40f;
	[Export] public Vector2 ChompTime = new Vector2(3.4f, 5.5f);
	[ExportGroup("Tongue")]
	[Export] public float TongueSpeed = 2.3f;
    [Export] public float TongueAngle = 30f;

	private float blinkTime = 0f;
	private float chompTime = 0f;
	private float tongueTime = 0f;

	private float mouthAngle = 0f;

	private bool needsUpdating = false;

	private Vector3 firstTongueRotation = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		foreach (Node3D part in BodyParts)
		{
			part.Scale = Vector3.Zero;
		}
		firstTongueRotation = Tongue.RotationDegrees;
		mouthAngle = ClosedAngle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (Node3D part in BodyParts)
		{
			part.Scale = part.Scale.Lerp(Active ? Vector3.One : Vector3.Zero, (float)delta * PartSpeed);
        }

		if (Active)
		{
			tongueTime += (float)delta;
			Tongue.RotateX(Mathf.DegToRad(Mathf.Cos(tongueTime * TongueSpeed) * TongueAngle * (float)delta));
        }

		chompTime -= (float)delta;
        if (chompTime <= 0f || !Active)
        {
            mouthAngle = Mathf.Lerp(mouthAngle, ClosedAngle, (float)delta * ChompSpeed);
            Mouth.RotationDegrees = new Vector3(mouthAngle, 0f, 0f);
            if (Mathf.Abs(mouthAngle - ClosedAngle) <= 1f)
                chompTime = (float)GD.RandRange(ChompTime.X, ChompTime.Y);
        }
        if (chompTime > 0f && Active)
        {
            mouthAngle = Mathf.Lerp(mouthAngle, OpenAngle, (float)delta * ChompSpeed);
            Mouth.RotationDegrees = new Vector3(mouthAngle, 0f, 0f);
        }
    }

    public void Activate()
    {
        Active = true;
        RandomizeEyes();
    }

    public void Deactivate()
    {
        Active = false;
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
