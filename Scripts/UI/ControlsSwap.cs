using Godot;
using System;

public partial class ControlsSwap : TextureRect
{
    [ExportCategory("Textures")]
    [Export] public Texture2D MouseIcon;
    [Export] public Texture2D ControllerIcon;
    [ExportCategory("Visual")]
    [Export] public bool Rotates = false;
    [Export] public Label text;

    public override void _Ready()
    {
        PivotOffset = (Size / 2);
    }

    public void SetController()
    {
        Texture = ControllerIcon;
        if (Rotates)
        {
            RotationDegrees = 0f;
            text.RotationDegrees = -45f;
        }
    }

    public void SetMouse()
    {
        Texture = MouseIcon;
        if (Rotates)
        {
            RotationDegrees = -45f;
            text.RotationDegrees = 0f;
        }
    }
}
