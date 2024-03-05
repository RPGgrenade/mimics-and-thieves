using Godot;
using System;

public partial class Barrier : Node3D
{
    [Export] public Node3D Visual;
    [Export] public float SpinSpeed = 4f;
    [Export] public float SizeSpeed = 4f;
    [Export] public float Time = 8f;

	private float time = 0f;
	public ThiefController controller;

	public override void _Ready()
	{
		time = Time;
		Visual.Scale = Vector3.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time -= (float)delta;
		Visual.RotationDegrees += new Vector3(0f, (float)delta * SpinSpeed, 0f);
		controller.IsProtected = true;

		if (time > 0f)
		{
			if (Visual.Scale < Vector3.One)
				Visual.Scale += Vector3.One * (float)delta * SizeSpeed;
			else Visual.Scale = Vector3.One;
		}
		else
        {
			if (Visual.Scale > Vector3.Zero)
				Visual.Scale -= Vector3.One * (float)delta * SizeSpeed;
			else
            {
                controller.IsProtected = false;
                QueueFree(); 
			}
        }
	}
}
