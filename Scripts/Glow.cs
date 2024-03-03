using Godot;
using System;
using System.Reflection;

public partial class Glow : Node3D
{
	private float duration = 0f;

	private float speed = 4f;

	public override void _Ready()
	{
		Scale = Vector3.Zero;
	}

	public override void _Process(double delta)
	{
		if (duration > 0f)
		{
			duration -= (float)delta;
			Scale = Scale.Lerp(Vector3.One, (float)delta * speed);
		}
		else Scale = Scale.Lerp(Vector3.Zero, (float)delta * speed);
    }

	public void SetGlowTime(float time)
	{
		duration = time;
	}
}
