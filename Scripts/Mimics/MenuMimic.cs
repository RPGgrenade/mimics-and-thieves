using Godot;
using System;

public partial class MenuMimic : Node3D
{

    [ExportCategory("Blink References")]
    [Export] public float BlinkSpeed = 2.5f;
    [Export] public Vector2 BlinkTime = new Vector2(0.4f, 3f);

	private float blinkTime = 0f;
    private bool closing = false;

    private float blink = 0f;

    public override void _Ready()
    {
        Scale = new Vector3(1f,0f,1f);
        blinkTime = (float)GD.RandRange(BlinkTime.X, BlinkTime.Y);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        blinkTime -= (float)delta;
        if (blinkTime <= 0f)
        {
            Visible = true;
            if (blink < 1f && !closing)
                blink += (float)delta * BlinkSpeed;
            if (blink >= 1f && !closing)
            {
                blink = 1f;
                closing = true;
            }

            if (blink > 0f && closing)
                blink -= (float)delta * BlinkSpeed;
            if (blink <= 0f && closing)
            {
                blink = 0f;
                closing = false;
                blinkTime = (float)GD.RandRange(BlinkTime.X, BlinkTime.Y);
            }
        }
        else { 
            blink = 0f; 
            Visible = false;
        }
        Scale = new Vector3(1f, blink, 1f);
	}
}
